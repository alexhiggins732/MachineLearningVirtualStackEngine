SET IDENTITY_INSERT FlowControl ON
    GO
	merge into Flowcontrol as Target
	using (select * from alexlaptop.ilruntime.dbo.flowcontrol)
	as source
	on Target.Id=source.Id

	when not matched by target then
	insert (id,name, value, description)
	values(source.id, source.name, source.value, source.description);

    SET IDENTITY_INSERT FlowControl OFF
    GO 