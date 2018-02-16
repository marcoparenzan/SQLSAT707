CREATE PROCEDURE [dbo].[ImportTemperatureEvent](
	@DeviceId nvarchar(32),
	@Timestamp datetimeoffset,
	@Temperature float
)
AS
	INSERT INTO TemperatureEvents (DeviceId, Timestamp, Temperature)
	VALUES (@DeviceId, @Timestamp, @Temperature)

