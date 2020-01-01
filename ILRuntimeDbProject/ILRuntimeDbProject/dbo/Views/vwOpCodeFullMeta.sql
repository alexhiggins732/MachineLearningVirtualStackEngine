CREATE VIEW [dbo].[vwOpCodeFullMeta]
AS
select 
	ClrName,
	OpCode=oc.Name,
	OpCodeValue=oc.Value,
	OpCodeSize=Size,
	-- FlowControl
	FlowControl=fc.Name,
	FlowControlValue=fc.Value,

	-- OpCodeType
	OpCodeType= oct.Name,
	OpCodeTypeValue= oct.Value,

	-- OperandType
	OperandType= ot.Name,
	OperandTypeValue= ot.Value,
	OperandTypeBitSize= ot.BitSize,
	OperandTypeByteSize= ot.ByteSize,
	OperandTypeIsFloatingPoint= ot.IsFloatingPoint,
	OperandTypeSystemType= ot.SystemType,
	-- StackBehaviourPop
	StackBehaviourPop= sbpop.Name,
	StackBehaviourPopValue= sbpop.Value,
	StackBehaviourPopCount = sbpop.PopCount,
	StackBehaviourPopType0 = sbpop.PopType0,
	StackBehaviourPopType1 = sbpop.PopType1,
	StackBehaviourPopType2 = sbpop.PopType2,

	-- StackBehaviourPush
	StackBehaviourPush= sbpush.Name,
	StackBehaviourPushValue= sbpush.Value,
	StackBehaviourPushCount = sbpush.PushCount,
	StackBehaviourPushType0= sbpush.PushType0,	
	StackBehaviourPushType1= sbpush.PushType1,	
	-- Descriptions	
	OpCodeDescription = oc.Description,
	FlowControlDescription=fc.Description,
	OpCodeTypeDescription = oct.Description,
	OperandTypeDescription= ot.Description,
	StackBehaviourPopDescription= sbpop.Description,
	StackBehaviourPushDescription= sbpush.Description,

	--IDs
	-- Descriptions	
	OpCodeId = oc.Id,
	FlowControlId=fc.Id,
	OpCodeTypeId = oct.Id,
	OperandTypeId= ot.Id,
	StackBehaviourPopId= sbpop.Id,
	StackBehaviourPushId= sbpush.Id
--,* 
from 
	opcode oc
	join flowcontrol fc on oc.flowcontrolid=fc.id
	join OpCodeType oct on oc.OpCodeTypeId=oct.id
	join OperandType ot on oc.OperandTypeId=ot.id
	join StackBehaviour sbpop on oc.StackBehaviourPopId=sbpop.id
	join StackBehaviour sbpush on oc.StackBehaviourPushId=sbpush.id
