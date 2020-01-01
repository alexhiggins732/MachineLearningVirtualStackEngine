CREATE View vwOpCode

As

select 
	ClrName,
	oc.Name,
	oc.Value,
	Size,
	FlowControl=fc.Name,
	OpCodeType= oct.Name,
	OperandType= ot.Name,
	StackBehaviourPop= sbpop.Name,
	StackBehaviourPush= sbpush.Name
--,* 
from 
	opcode oc
	join flowcontrol fc on oc.flowcontrolid=fc.id
	join OpCodeType oct on oc.OpCodeTypeId=oct.id
	join OperandType ot on oc.OperandTypeId=ot.id
	join StackBehaviour sbpop on oc.StackBehaviourPopId=sbpop.id
	join StackBehaviour sbpush on oc.StackBehaviourPushId=sbpush.id