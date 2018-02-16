CREATE PROCEDURE [dbo].[ImportTemperatureJSONEvent] (
	@json nvarchar(MAX)
)
AS
	INSERT INTO TemperatureEvents (DeviceId, Timestamp, Temperature)
	SELECT JSON_VALUE(@json, '$.DeviceId'), JSON_VALUE(@json, '$.Timestamp'), JSON_VALUE(@json, '$.Temperature')

