using Microsoft.Data.SqlClient;



/// <summary>
/// Summary description for TransactionManager.
/// </summary>
public class TransactionManager
{

    private SqlConnection conn;
    private SqlTransaction trans;

    private string _cnstring;
    private bool _transactionOpen = false;

    /// <summary>
    ///		Initializes the object.
    /// </summary>
    /// <param name="ConnectionString">Connection string to datasource.</param>
    public TransactionManager(string ConnectionString)
    {
        _cnstring = ConnectionString;
        conn = new SqlConnection(ConnectionString);
    }
    /// <summary>
    ///		Connection string to the datasource.
    ///		Do not change during a transaction.
    /// </summary>
    public string ConnectionString {
        get { return _cnstring; }
        set {//make sure transaction is open
            if (IsOpen)
                throw new DBException("Connection string cannot be changed during a transaction");
            conn.ConnectionString = value;
        }
    }

    /// <summary>
    ///		Allows for objects to access the OracleTransaction object.
    /// </summary>
    internal SqlTransaction TransactionObject {
        get { return trans; }
    }

    //		/// <summary>
    //		///		Return true if a transaction session is currently open and operating.
    //		/// </summary>
    //		public bool TransactionOpen 
    //		{
    //			get { return _transactionOpen; }
    //		}
    /// <summary>
    ///		Return true if a transaction session is currently open and operating.
    /// </summary>
    public bool IsOpen {
        get { return _transactionOpen; }
    }
    /// <summary>
    ///		Begins a transaction.
    /// </summary>
    public void BeginTransaction()
    {
        if (IsOpen)
            throw new DBException("Transaction already open.");
        //Open connection
        try
        {
            conn.Open();
            trans = conn.BeginTransaction();
            this._transactionOpen = true;
        }
        // in the event of an error, close the connection and destroy the transobject.
        catch (Exception e)
        {
            conn.Close();
            trans.Dispose();
            throw e;
        }
    }

    /// <summary>
    ///		Commit the transaction to the datasource.
    /// </summary>
    public void Commit()
    {
        if (!this.IsOpen)
            throw new DBException("Transaction needs to begin first.");
        trans.Commit();
        //assuming the commit was sucessful.
        this.conn.Close();
        trans.Dispose();
        this._transactionOpen = false;
    }

    /// <summary>
    ///		
    /// </summary>
    public void Rollback()
    {
        if (!this.IsOpen)
            throw new DBException("Transaction needs to begin first.");
        trans.Rollback();
        conn.Close();
        trans.Dispose();
        this._transactionOpen = false;
    }
}//end class

