-- Insert rows into table 'EventTypeModels' in schema '[dbo]'
INSERT INTO [dbo].[EventTypeModels]
( -- Columns to insert data into
 Name
)
VALUES
( -- First row: values for the columns in the list above
 'Manoton'
)

GO

-- Insert rows into table 'EventModels' in schema '[dbo]'
INSERT INTO [dbo].[EventModels]
( -- Columns to insert data into
 Title, Description, StartDate, EndDate, EventTypeId
)
VALUES
( -- First row: values for the columns in the list above
 'Manoton 1', 'Descripcion de este Manoton 1', '20190822 09:00:00 AM','20190822 12:00:00 AM', 1
),
( -- Second row: values for the columns in the list above
 'Manoton 2', 'Descripcion de este Manoton 2', '20190828 09:00:00 AM','20190828 12:00:00 AM', 1
),
(
 'Manoton 3', 'Descripcion de este Manoton 3', '20190904 09:00:00 AM',' 20190904 12:00:00 AM', 1
)
-- Add more rows here
GO