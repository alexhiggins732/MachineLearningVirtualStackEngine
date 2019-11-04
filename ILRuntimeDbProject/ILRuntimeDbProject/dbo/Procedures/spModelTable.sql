CREATE PROCEDURE [dbo].[spModelTable]
	@tableName varchar(100) 
AS
select
case 
	when c.system_type_id=167 then 'public string ' + c.name  + ' { get; set; }'
	when c.system_type_id=56 then 'public int ' + c.name  + ' { get; set; }'
	when c.system_type_id=104 then 'public bool ' + c.name  + ' { get; set; }'
	else 'unknown'
end
,
* from sys.columns c join sys.tables tbl
on c.object_id=tbl.object_id where tbl.name=@tableName