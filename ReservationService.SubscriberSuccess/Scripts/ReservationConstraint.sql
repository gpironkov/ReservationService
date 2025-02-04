-- Add computed columns only if they do not already exist
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE name = 'TableNumber' AND object_id = OBJECT_ID('Reservations')
)
BEGIN
    ALTER TABLE Reservations
    ADD TableNumber AS JSON_VALUE(RawRequest, '$.NumberOfReservedTable') PERSISTED;
END

IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE name = 'ReservationDate' AND object_id = OBJECT_ID('Reservations')
)
BEGIN
    ALTER TABLE Reservations
    ADD ReservationDate AS JSON_VALUE(RawRequest, '$.DateOfReservation') PERSISTED;
END

-- Add the UNIQUE constraint only if it doesn't exist
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'UQ_Table_Reservation' AND object_id = OBJECT_ID('Reservations')
)
BEGIN
    ALTER TABLE Reservations
    ADD CONSTRAINT UQ_Table_Reservation UNIQUE (TableNumber, ReservationDate);
END
