using System;
using System.Data;
using System.Data.Common;


public class DataReaderAdapter  : DbDataAdapter 
{ 
	public  int FillFromReader(DataTable dataTable, IDataReader dataReader,int startRecord,int MaxRecords) 
	{ 
		DataSet tempDataSet=new DataSet();
		tempDataSet.Tables.Add(dataTable);
		return  this.Fill(tempDataSet, tempDataSet.Tables[0].TableName, dataReader,startRecord,MaxRecords); 
	} 

	public  int FillFromReader(DataTable dataTable, IDataReader dataReader) 
	{ 
			
		return  this.Fill(dataTable, dataReader); 
	} 

	protected override RowUpdatedEventArgs CreateRowUpdatedEvent( 
		DataRow dataRow, 
		IDbCommand command, 
		StatementType statementType, 
		DataTableMapping tableMapping 
		){return null;} 

	protected override RowUpdatingEventArgs CreateRowUpdatingEvent( 
		DataRow dataRow, 
		IDbCommand command, 
		StatementType statementType, 
		DataTableMapping tableMapping 
		){return null;} 

	protected override void OnRowUpdated( 
		RowUpdatedEventArgs value 
		){} 
	protected override void OnRowUpdating( 
		RowUpdatingEventArgs value 
		){} 
} 


public class DBException : ApplicationException
{
	public DBException(string message) : base(message)
	{

	}
	

}

public class DBConcurrencyException : DBException
{

	private object _modifiedrecord;
	private object _dsrecord;
	public DBConcurrencyException(string message):base(message) {} 

	public DBConcurrencyException(string message, object modifiedrecord, object dsrecord):base(message)
	{	
		//this.Message = message;
		this.ModifiedRecord = modifiedrecord;
		this.DatasourceRecord = dsrecord;
	}
	
	/// <summary>
	///		The modified object to be updated.
	/// </summary>
	public object ModifiedRecord
	{
		get { return _modifiedrecord; }
		set { _modifiedrecord = value; }
	}

	/// <summary>
	///		The current record in the database.
	/// </summary>
	public object DatasourceRecord
	{
		get { return _dsrecord; }
		set { _dsrecord = value; }
	}


}




 
	

