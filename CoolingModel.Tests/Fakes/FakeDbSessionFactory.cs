using System.Data;
using System.Data.Common;
using HSM_CommonCS.Database;

namespace CoolingModel.Tests.Fakes;

/// <summary>
/// Fake database session factory for testing.
/// Returns an in-memory connection that can serve pre-configured result sets.
/// No Oracle dependency needed.
/// </summary>
public class FakeDbSessionFactory : IDbSessionFactory
{
    private readonly List<FakeResultSet> _resultSets = new();
    private int _openCount;
    private bool _disposed;

    /// <summary>How many times Open() was called.</summary>
    public int OpenCount => _openCount;

    /// <summary>
    /// Queue a result set that will be returned by the next query execution.
    /// Result sets are consumed in FIFO order.
    /// </summary>
    public void EnqueueResultSet(FakeResultSet resultSet)
    {
        _resultSets.Add(resultSet);
    }

    /// <summary>
    /// Convenience: queue a result set from a DataTable.
    /// </summary>
    public void EnqueueResultSet(DataTable table)
    {
        _resultSets.Add(new FakeResultSet(table));
    }

    public DbConnection Open()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(FakeDbSessionFactory));
        _openCount++;
        return new FakeDbConnection(_resultSets);
    }

    public void Dispose()
    {
        _disposed = true;
    }
}

/// <summary>
/// Wraps a DataTable as a returnable result set.
/// </summary>
public class FakeResultSet
{
    public DataTable Data { get; }

    public FakeResultSet(DataTable data)
    {
        Data = data;
    }
}

/// <summary>
/// Minimal fake DbConnection that returns fake commands.
/// </summary>
public class FakeDbConnection : DbConnection
{
    private readonly List<FakeResultSet> _resultSets;
    private ConnectionState _state = ConnectionState.Closed;

    public FakeDbConnection(List<FakeResultSet> resultSets)
    {
        _resultSets = resultSets;
    }

    public override string ConnectionString { get; set; } = "FakeConnection";
    public override string Database => "FakeDb";
    public override string DataSource => "FakeSource";
    public override string ServerVersion => "1.0";
    public override ConnectionState State => _state;

    public override void Open() => _state = ConnectionState.Open;
    public override Task OpenAsync(CancellationToken ct)
    {
        _state = ConnectionState.Open;
        return Task.CompletedTask;
    }
    public override void Close() => _state = ConnectionState.Closed;
    public override void ChangeDatabase(string databaseName) { }

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
        new FakeDbTransaction(this);

    protected override DbCommand CreateDbCommand() =>
        new FakeDbCommand(_resultSets);
}

/// <summary>
/// Fake command that returns data from queued result sets.
/// </summary>
public class FakeDbCommand : DbCommand
{
    private readonly List<FakeResultSet> _resultSets;
    private readonly DbParameterCollection _parameters = new FakeDbParameterCollection();

    public FakeDbCommand(List<FakeResultSet> resultSets)
    {
        _resultSets = resultSets;
    }

    public override string CommandText { get; set; } = string.Empty;
    public override int CommandTimeout { get; set; }
    public override CommandType CommandType { get; set; }
    public override UpdateRowSource UpdatedRowSource { get; set; }
    protected override DbConnection? DbConnection { get; set; }
    protected override DbTransaction? DbTransaction { get; set; }
    public override bool DesignTimeVisible { get; set; }

    protected override DbParameterCollection DbParameterCollection => _parameters;

    public override void Cancel() { }
    public override void Prepare() { }

    public override int ExecuteNonQuery() => 0;

    public override object? ExecuteScalar()
    {
        if (_resultSets.Count == 0) return null;
        var rs = _resultSets[0];
        _resultSets.RemoveAt(0);
        if (rs.Data.Rows.Count == 0) return null;
        return rs.Data.Rows[0][0];
    }

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        if (_resultSets.Count == 0)
            return new DataTableReader(new DataTable()); // empty

        var rs = _resultSets[0];
        _resultSets.RemoveAt(0);
        return new DataTableReader(rs.Data);
    }

    protected override DbParameter CreateDbParameter() => new FakeDbParameter();
}

public class FakeDbParameter : DbParameter
{
    public override DbType DbType { get; set; }
    public override ParameterDirection Direction { get; set; }
    public override bool IsNullable { get; set; }
    public override string ParameterName { get; set; } = string.Empty;
    public override int Size { get; set; }
    public override string SourceColumn { get; set; } = string.Empty;
    public override bool SourceColumnNullMapping { get; set; }
    public override object? Value { get; set; }
    public override void ResetDbType() { }
}

public class FakeDbParameterCollection : DbParameterCollection
{
    private readonly List<DbParameter> _params = new();

    public override int Count => _params.Count;
    public override object SyncRoot => _params;

    public override int Add(object value) { _params.Add((DbParameter)value); return _params.Count - 1; }
    public override void AddRange(Array values) { foreach (DbParameter p in values) _params.Add(p); }
    public override void Clear() => _params.Clear();
    public override bool Contains(object value) => _params.Contains(value);
    public override bool Contains(string value) => _params.Any(p => p.ParameterName == value);
    public override void CopyTo(Array array, int index) => ((System.Collections.ICollection)_params).CopyTo(array, index);
    public override int IndexOf(object value) => _params.IndexOf((DbParameter)value);
    public override int IndexOf(string parameterName) => _params.FindIndex(p => p.ParameterName == parameterName);
    public override void Insert(int index, object value) => _params.Insert(index, (DbParameter)value);
    public override void Remove(object value) => _params.Remove((DbParameter)value);
    public override void RemoveAt(int index) => _params.RemoveAt(index);
    public override void RemoveAt(string parameterName) => _params.RemoveAll(p => p.ParameterName == parameterName);

    protected override DbParameter GetParameter(int index) => _params[index];
    protected override DbParameter GetParameter(string parameterName) => _params.First(p => p.ParameterName == parameterName);
    protected override void SetParameter(int index, DbParameter value) => _params[index] = value;
    protected override void SetParameter(string parameterName, DbParameter value)
    {
        var idx = IndexOf(parameterName);
        if (idx >= 0) _params[idx] = value;
        else _params.Add(value);
    }

    public override IEnumerator<object> GetEnumerator() => _params.Cast<object>().GetEnumerator();
}

public class FakeDbTransaction : DbTransaction
{
    public FakeDbTransaction(DbConnection connection) => DbConnection = connection;
    protected override DbConnection? DbConnection { get; }
    public override IsolationLevel IsolationLevel => IsolationLevel.Unspecified;
    public override void Commit() { }
    public override void Rollback() { }
}
