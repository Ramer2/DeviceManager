-- Add SmartWatches
EXEC AddSmartWatch @DeviceId = 'SW-1', @Name = 'Apple Watch Series 6', @IsOn = 0, @BatteryCharge = 85;
EXEC AddSmartWatch @DeviceId = 'SW-2', @Name = 'Samsung Galaxy Watch', @IsOn = 1, @BatteryCharge = 60;

-- Add Personal Computers
EXEC AddPersonalComputer @DeviceId = 'P-1', @Name = 'Dell XPS 13', @IsOn = 1, @OperatingSystem = 'Windows 11';
EXEC AddPersonalComputer @DeviceId = 'P-2', @Name = 'Lenovo ThinkPad', @IsOn = 0, @OperatingSystem = 'Ubuntu 22.04';

-- Add Embedded Devices
EXEC AddEmbeddedDevice @DeviceId = 'ED-1', @Name = 'Raspberry Pi 4', @IsOn = 1, @IpAddress = '192.168.0.10', @NetworkName = 'HomeNetwork', @IsConnected = 1;
EXEC AddEmbeddedDevice @DeviceId = 'ED-2', @Name = 'ESP32 Sensor', @IsOn = 1, @IpAddress = '192.168.0.11', @NetworkName = 'OfficeNetwork', @IsConnected = 1;
