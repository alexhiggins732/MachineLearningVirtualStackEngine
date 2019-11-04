CREATE PROCEDURE [dbo].[spModelView]
	@viewName varchar(100) 
AS
select
case 
	when c.system_type_id=167 then 'public string ' + c.name  + ' { get; set; }'
	when c.system_type_id=56 then 'public int ' + c.name  + ' { get; set; }'
	when c.system_type_id=104 then 'public bool ' + c.name  + ' { get; set; }'
	else 'unknown'
end
,
* from sys.columns c join sys.views vw
on c.object_id=vw.object_id where vw.name=@viewName