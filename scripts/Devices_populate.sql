-- Insert data into Device table
INSERT INTO Device (Id, Name, IsOn) VALUES
                ('ED-1', 'Raspberry Pi 4', 1),
                ('P-1', 'Dell XPS 13', 1),
                ('SW-1', 'Apple Watch Series 6', 0),
                ('ED-2', 'ESP32 Sensor', 1),
                ('P-2', 'Lenovo ThinkPad', 0),
                ('SW-2', 'Samsung Galaxy Watch', 1);

-- Insert data into EmbeddedDevice table
INSERT INTO EmbeddedDevice (Id, IpAddress, NetworkName, IsConnected, Device_id) VALUES
                (1, '192.168.0.10', 'HomeNetwork', 1, 'ED-1'),
                (2, '192.168.0.11', 'OfficeNetwork', 1, 'ED-2');

-- Insert data into PersonalComputer table
INSERT INTO PersonalComputer (Id, OperatingSystem, Device_id) VALUES
                  (1, 'Windows 11', 'P-1'),
                  (2, 'Ubuntu 22.04', 'P-2');

-- Insert data into SmartWatch table
INSERT INTO SmartWatch (Id, BatteryCharge, Device_id) VALUES
                  (1, 85, 'SW-1'),
                  (2, 60, 'SW-2');
