# About

Adoler is a SQL helper that helps to connect, execute SQL commands, call the SQL stored procedures, and retrieve
Directly insert the selected data into your objects.

You can pass your parameters using a direct DTO, or Data Model Dynamic Object, or pass your parameters directly.

It is easy to use the output parameters, multiple record sets, and dynamic objects.


# How to Use
```

public async Task<bool> UpdateAsync(ArticleDto dto)
{
    List<SqlParameter> plist = dto.ConvertToParametersExcept(e => e.CreatedBy, e => e.CreationDate, e => e.IsAvailable);
    int affectedRows = dbHelper.SqlHelperWrite.ExecuteNonQuery("[dbo].[Document_Update]", plist);
    return (affectedRows > 0);
}


public async Task<bool> DeleteAsync(int id, string user)
{
    dynamic parameters = new ExpandoObject();
    parameters.ArticleId = id;
    parameters.LastModifiedBy = user;
    int affectedRows = dbHelper.SqlHelperWrite.ExecuteNonQuery("[dbo].[Article_Delete]", parameters);
    return (affectedRows > 0);
}


public async Task<List<ArticleInfoDto>> GetAllAsync(EnumLanguage lang = EnumLanguage.English, int? categoryId = null)
{
    dynamic parameters = new ExpandoObject();
    parameters.LangId = (int)lang;
    parameters.CategoryId = categoryId;
    List<ArticleInfoDto> list = dbHelper.SqlHelperRead.GetList<ArticleInfoDto>("[dbo].[Article_SelectAll]", parameters);
    return list;
}
```	

# Key Features

* Object to SQL Parameters
* Dynamic Objects to SQL Parameters
* SQL Result into Generic objects
