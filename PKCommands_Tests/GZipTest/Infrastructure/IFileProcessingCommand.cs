using System;
using PK.Commands.CommandProcessing;
using PK.Commands.Interfaces;

namespace GZipTest.Infrastructure;

/// <summary> Interface for general GZipTest file processing command. </summary>
public interface IFileProcessingCommand : ICommandEx<ExitCode>, IFileProcessingQueue, IDisposable
{
}
