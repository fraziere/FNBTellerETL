using FNBCoreETL.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Model
{
    interface IETLModel : ICommand, IDelegate, IMode, IArgument, ISearch
    {
    }

    #region Fluent Interfaces
    public interface ICommand
    {
        IMode AddCommand(string command, string name, string description);
    }

    public interface IMode : IChain
    {
        IDelegate AddDelegate(Func<ETLJobIn, ETLJobOut> method);
    }

    public interface IChain
    {
        IChain AddChain(string command, string modeCommand, bool continueOnError = true);
    }

    public interface IDelegate
    {
        IArgument AddMode<T>(string command, T name, string description) where T : System.Enum;
    }

    public interface IArgument
    {
        IArgument AddMode<T>(string command, T name, string description) where T : System.Enum;
        IArgument AddArgument(string name, string argDescription, Type argType, bool isRequired = true);
    }

    internal interface ISearch
    {
        IReadOnlyList<ETLJobIn> GetJobs(string cmd, string modeCmd, IReadOnlyList<string> args);
    }

    #endregion
}
