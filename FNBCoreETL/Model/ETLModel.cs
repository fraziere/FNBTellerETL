using FNBCoreETL.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Model
{
    public sealed class ETLModel : IETLModel
    {
        private static readonly ETLModel model = new ETLModel();

        internal static ICommand Model { get { return model; } }
        internal static ISearch Search { get { return model; } }

        private Dictionary<string, ETLCommand> _etlCommands;
        private Dictionary<string, ETLChainCommand> _etlChainCommands;

        private ETLCommand _currentCommand;
        private ETLMode _currentMode;

        static ETLModel() { }
        private ETLModel() {
            _etlCommands = new Dictionary<string, ETLCommand>();
            _etlChainCommands = new Dictionary<string, ETLChainCommand>();
        }

        public IMode AddCommand(string command, string name, string description)
        {
            var cmd = new ETLCommand(command, name, description);
            cmd.IsValid();

            if (_etlCommands.Values.Any(x => x.Command.Equals(cmd.Command)))
            {
                throw new ETLModelException($"Command ({cmd.Command}) already exists in the model. Note: case insensitive.");
            }

            model._etlCommands.Add(cmd.Command, cmd);
            this._currentCommand = model._etlCommands[cmd.Command];
            return model;
        }

        public IDelegate AddDelegate(Func<ETLJobIn, ETLJobOut> method)
        {
            model._etlCommands[_currentCommand.Command].AddDelegate(method);
            return model;
        }
        public IArgument AddMode<T>(string modeCommand, T modeName, string modeDescription) where T: System.Enum
        {
            var mode = new ETLMode(modeCommand, modeName.ToString(), modeDescription);
            mode.IsValid();

            if (_currentCommand.Modes.Any(x=>x.Command.Equals(mode.Command)))
            {
                throw new ETLModelException($"Mode command: ({mode.Command}) already exists in the model for base command: ({_currentCommand.Command}). Note: case insensitive.");
            }

            model._etlCommands[_currentCommand.Command].AddMode(mode);
            this._currentMode = model._etlCommands[_currentCommand.Command].Modes.Where(x => x.Command.Equals(mode.Command)).Single();
            return model;
        }

        public IChain AddChain(string command, string modeCommand, bool continueOnError = true)
        {

            if (_etlCommands.ContainsKey(command.ToLower()) == false)
            {
                throw new ETLModelException($"Could not find command ({command.ToLower()}) to add to chaining command ({ _currentCommand.Command }).  Please ensure command/mode is added to model prior to setting up a chain.");
            }

            var foundCmd = _etlCommands[command.ToLower()];
            if (foundCmd.Modes.Any(x => x.Command.Equals(modeCommand.ToLower())) == false)
            {
                throw new ETLModelException($"Could not find mode ({modeCommand.ToLower()}) to add to chaining command ({ _currentCommand.Command }).  Please ensure command/mode is added to model prior to setting up a chain.");
            }

            var foundMode = foundCmd.Modes.Where(x => x.Command.Equals(modeCommand.ToLower())).Single();
            if (foundMode.ArgumentsByOrder.Count() > 0)
            {
                throw new ETLModelException($"Could not add add command and mode to the chaining command ({ _currentCommand.Command }).  Doesn't currently support chaining jobs w/ arguments.  Yeah, I'll get to it.");
            }

            if (_etlChainCommands.ContainsKey(_currentCommand.Command))
            {
                _etlChainCommands[_currentCommand.Command].AddCommandWithMode(foundCmd.Command, foundCmd.Name, foundCmd.Description, foundCmd.Method, foundMode, continueOnError);
            }
            else
            {
                var chainCmd = new ETLChainCommand(_currentCommand.Command, _currentCommand.Name, _currentCommand.Description);
                chainCmd.AddCommandWithMode(foundCmd.Command, foundCmd.Name, foundCmd.Description, foundCmd.Method, foundMode, continueOnError);
                _etlChainCommands.Add(chainCmd.Command, chainCmd);
            }

            return model;
        }

        public IArgument AddArgument(string argName, string argDescription, Type argType, bool isRequired = true)
        {
            var newArg = new ETLArgument(argName, argDescription, argType, isRequired);

            if (_currentMode.ArgumentsByOrder.Values.Any(x => x.Name.Equals(newArg.Name)))
            {
                throw new ETLModelException($"Argument Name {newArg.Name} for command ({_currentCommand.Command}) and mode ({_currentMode.Command}) is already assigned.");
            }

            _currentMode.AddArgument(newArg);
            if (_currentMode.ArgumentsAreValid() == false)
            {
                throw new ETLModelException($"Arguments for command ({_currentCommand.Command}) and mode ({_currentMode.Command}) must have required arguments listed before optional.");
            }
            
            return model;
        }

        public IReadOnlyList<ETLJobIn> GetJobs(string cmd, string mode, IReadOnlyList<string> args)
        {
            var retVal = new List<ETLJobIn>();

            var isChain = _etlChainCommands.ContainsKey(cmd.ToLower());
            var isCmd = _etlCommands.ContainsKey(cmd.ToLower());

            if (isChain)
            {
                var etlChainCommand = _etlChainCommands[cmd.ToLower()];
                foreach (var etlCommand in etlChainCommand.CommandsByOrder.OrderBy(x => x.Key).Select(x => x.Value))
                {
                    foreach (var etlMode in etlCommand.Modes)
                    {
                        //TODO args in chain
                        retVal.Add(new ETLJobIn(true, etlChainCommand.Name, 
                            etlCommand.Command, etlCommand.Name,
                            etlMode.Command, etlMode.Name,
                            new Dictionary<string, string>(), etlCommand.Method, etlCommand.ContinueOnError));
                    }
                }
            }
            else if (isCmd)
            {
                var etlCommand = _etlCommands[cmd.ToLower()];
                var etlMode = etlCommand.Modes.Where(x => x.Command.Equals(mode.ToLower())).SingleOrDefault();
                if (etlMode == null)
                {
                    throw new ETLModelException($"Command not found  mode({mode ?? "NULL"}) for cmd({cmd})");
                }

                var etlArgs = new Dictionary<string, string>();

                if (etlMode.ArgumentsByOrder.Count() != args.Count())
                {
                    throw new ETLModelException("Count Mismatch for arguments"); //todo better error
                }
                int i = 0;
                foreach (var etlArg in etlMode.ArgumentsByOrder.OrderBy(x => x.Key).Select(x => x.Value))
                {
                    //TODO Cast checking
                    etlArgs.Add(etlArg.Name, args[i]);
                    i++;
                }

                retVal.Add(new ETLJobIn(false, String.Empty, 
                    etlCommand.Command, etlCommand.Name, 
                    etlMode.Command, etlMode.Name,
                    etlArgs, etlCommand.Method, false));
            }
            else
            {
                throw new ETLModelException($"Command not found for cmd({cmd ?? "NULL"}) mode({mode ?? "NULL"})");
            }

            return retVal;
        }
    }
}
