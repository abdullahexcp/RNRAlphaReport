
DECLARE @SearchString NVARCHAR(100) = N'صندوق الايرادات -فرع الدمام';
DECLARE @TableName NVARCHAR(256), @ColumnName NVARCHAR(128),@FoundValue NVARCHAR(3630), @SQL NVARCHAR(MAX);

IF OBJECT_ID('tempdb..##Results') IS NOT NULL DROP TABLE ##Results;
CREATE TABLE ##Results(TableName NVARCHAR(370), ColumnName NVARCHAR(370), FoundValue NVARCHAR(3630));

DECLARE ColumnCursor CURSOR FOR
SELECT TABLE_NAME, COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE DATA_TYPE IN ('char', 'varchar', 'nchar', 'nvarchar', 'text', 'ntext');

OPEN ColumnCursor;

FETCH NEXT FROM ColumnCursor INTO @TableName, @ColumnName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @SQL = 'IF EXISTS (SELECT [' + @ColumnName + '] FROM [' + @TableName + '] WHERE [' + @ColumnName + '] LIKE N''' + @SearchString + ''')
                BEGIN
                    DECLARE @MatchingValue NVARCHAR(3630);
                    SELECT TOP 1 @MatchingValue = [' + @ColumnName + '] FROM [' + @TableName + '] WHERE [' + @ColumnName + '] LIKE N''' + @SearchString + ''';
                    INSERT INTO ##Results (TableName, ColumnName, FoundValue) VALUES (''' + @TableName + ''', ''' + @ColumnName + ''', @MatchingValue);
                 END';
    EXEC sp_executesql @SQL;

    FETCH NEXT FROM ColumnCursor INTO @TableName, @ColumnName;
END

CLOSE ColumnCursor;
DEALLOCATE ColumnCursor;

SELECT * FROM ##Results;
DROP TABLE ##Results;


select * from VW_GlActMF