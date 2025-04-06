using System;
using System.Diagnostics;
using PK.PkUtils.Extensions;

namespace GZipTest.Infrastructure;

public class FileBlock
{
    #region Typedefs
    /// <summary>
    /// Enumeration representing the status of a file block.
    /// </summary>
    public enum BlockStatus
    {
        /// <summary> Represents the undefined status, which should never happen. </summary>
        Undefined,

        /// <summary> Represents the initial status, after allocating but before data reading. </summary>
        Initial,

        /// <summary> Represents the status after input data reading. </summary>
        ReadDone,

        /// <summary> Represents the block processed state. </summary>
        Processed,

        /// <summary> Represents the block written state. </summary>
        Written,
    }
    #endregion // Typedefs

    #region Fields
    public const int DefaultBlockSize = 16 * 1024;
    public const int MinimalBlockSize = 1;

    private byte[] _buffer;
    private int _actualSize;
    private BlockStatus _status;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> 
    /// Default constructor which initializes the block with the default buffer size.
    /// </summary>
    public FileBlock() : this(DefaultBlockSize) { }

    /// <summary> 
    /// Constructor that initializes the block with a specified buffer size.
    /// </summary>
    /// <param name="blockSize"> The size of the buffer. Must be at least <see cref="MinimalBlockSize"/>. </param>
    public FileBlock(int blockSize)
    {
        if (blockSize < MinimalBlockSize)
        {
            string errorMessage = $"Actual argument value '{blockSize}' is less than minimal value '{MinimalBlockSize}'.";
            throw new ArgumentOutOfRangeException(nameof(blockSize), errorMessage);
        }

        _status = BlockStatus.Initial;
        _buffer = new byte[blockSize];
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>   Gets the status of the block. </summary>
    public BlockStatus Status => _status;

    /// <summary>   Gets the size of the buffer. </summary>
    public int BufferLength => _buffer.Length;

    /// <summary>   Gets the buffer that holds the data of the block. </summary>
    public byte[] Buffer => _buffer;

    /// <summary>
    /// Gets the actual size of the data stored in the block. The setter is protected and can only be called from internal methods.
    /// </summary>
    public int ActualSize
    {
        get => _actualSize;
        set => _actualSize = value;
    }

    /// <summary> Gets or initializes the original file position. </summary>
    public long FilePosition { get; init; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Returns the underlying byte buffer.
    /// </summary>
    public byte[] GetUnderlyingBuffer() => _buffer;

    /// <summary>
    /// Changes the status of the block to the specified new status, if valid.
    /// </summary>
    /// <param name="newStatus"> The new status to set. </param>
    /// <returns> The current instance of the <see cref="FileBlock"/> after the status is changed. </returns>
    public FileBlock ChangeStatus(BlockStatus newStatus)
    {
        newStatus.CheckIsDefinedValue(nameof(newStatus));
        if (Status >= newStatus)
        {
            string errorMessage = $"Can't change existing status '{Status}' to '{newStatus}'.";
            Debug.Fail(errorMessage);
            throw new ArgumentException(errorMessage, nameof(newStatus));
        }
        else
        {
            _status = newStatus;
        }

        return this;
    }

    // <summary>
    /// Resizes the internal buffer to a new size and assigns the provided buffer.
    /// </summary>
    /// <param name="newSize"> The new size of the buffer. </param>
    /// <param name="newBuffer"> The new buffer to assign. </param>
    public void AssignNewBuffer(byte[] newBuffer, int newSize)
    {
        ArgumentNullException.ThrowIfNull(newBuffer);
        ArgumentOutOfRangeException.ThrowIfNegative(newSize);

        // Directly assign the buffer and copy the data
        _buffer = newBuffer;

        // Update the ActualSize to match the size of the compressed data
        ActualSize = newSize;
    }

    // <summary>
    /// Returns a string that represents the current file block including its status, file position, and actual data size.
    /// </summary>
    public override string ToString()
    {
        return $"Status: {Status}, FilePosition: {FilePosition}, ActualSize: {ActualSize}";
    }

    #endregion // Methods
}
