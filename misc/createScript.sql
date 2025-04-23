-- Device
CREATE TABLE Devices (
                         Id VARCHAR(100) PRIMARY KEY,
                         Name VARCHAR(255) NOT NULL,
                         IsOn BIT NOT NULL
);

-- EmbeddedDevice
CREATE TABLE EmbeddedDevices (
                                 Id VARCHAR(100) PRIMARY KEY,
                                 IpAddress VARCHAR(15) NOT NULL,
                                 NetworkName VARCHAR(255) NOT NULL,
                                 IsConnected BIT NOT NULL,
                                 FOREIGN KEY (Id) REFERENCES Devices(Id)
);

-- PersonalComputer
CREATE TABLE PersonalComputers (
                                   Id VARCHAR(100) PRIMARY KEY,
                                   OperatingSystem VARCHAR(255) NOT NULL,
                                   FOREIGN KEY (Id) REFERENCES Devices(Id)
);

-- SmartWatch
CREATE TABLE SmartWatches (
                              Id VARCHAR(100) PRIMARY KEY,
                              BatteryCharge INT NOT NULL CHECK (BatteryCharge BETWEEN 0 AND 100),
                              FOREIGN KEY (Id) REFERENCES Devices(Id)
);
