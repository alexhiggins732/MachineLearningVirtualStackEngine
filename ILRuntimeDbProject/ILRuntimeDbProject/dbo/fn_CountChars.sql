CREATE FUNCTION [dbo].[fn_CountChars]
(
	-- Add the parameters for the function here
	@value varchar(100),
	@match varchar(1)
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
DECLARE @Result int =0

	declare @idx int = 0
	declare @len int = len(@value)
	while (@idx<=@len)
	begin
		if (substring(@value,@idx, 1) = @match)
		begin
			set @result = @result + 1
		end
		set @idx = @idx+1
	end
	-- Add the T-SQL statements to compute the return value here

	-- Return the result of the function
	return @Result

END
GO
