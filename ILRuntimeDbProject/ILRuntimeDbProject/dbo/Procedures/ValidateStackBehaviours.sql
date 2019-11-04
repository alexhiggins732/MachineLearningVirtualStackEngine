CREATE PROCEDURE [dbo].[ValidateStackBehaviours]
As 

SELECT
 
--IsValid=case when name='pop0' then 
--			case when popcount=0 and poptype0='' and poptype1='' and poptype2='' and pushcount=0 and pushtype0='' and pushtype1='' then 1 else 0 end
--		else -1
--		end

IsValidPop = 
case when name like '%pop%' then
	case when pushcount !=0 then 0 
	else
		case when name='Pop0' then
			case when popcount=0 then 1 else 0 end
		else
			case when dbo.fn_countchars(name, '_')+1 = popcount then 1 else 0 end
		end
	end
else 
	case when PopCount=0 then 1 else 0 end
	end
,
IsValidPopArgs =
	case 
		when popcount=0 then 
			case when poptype0 = '' and poptype1 ='' and poptype2 ='' then 1 else 0 end
		when popcount=1 then 
			case when poptype0 != '' and poptype1 ='' and poptype2 ='' then 1 else 0 end
		when popcount=2 then 
			case when poptype0 != '' and poptype1 !='' and poptype2 ='' then 1 else 0 end
		when popcount=3 then 
			case when poptype0 != '' and poptype1 !='' and poptype2 !='' then 1 else 0 end
		else 0
	end
,
IsValidPush = 
case when name like '%push%' then
	case when popcount !=0 then 0 
	else
		case when name='Push0' then
			case when pushcount=0 then 1 else 0 end
		else
			case when dbo.fn_countchars(name, '_')+1 = pushcount then 1 else 0 end
		end
	end
else 
	case when PushCount = 0 then 1 else 0 end
	end
,
IsValidPushArgs =
	case 
		when pushcount = 0 then 
			case when pushtype0 = '' and pushtype1 ='' then 1 else 0 end
		when pushcount = 1 then 
			case when pushtype0 != '' and pushtype1 ='' then 1 else 0 end
		when pushcount = 2 then 
			case when pushtype0 != '' and pushtype1 !='' then 1 else 0 end
		else 0
	end
,

*
  FROM [ILRuntime].[dbo].[StackBehaviour]

