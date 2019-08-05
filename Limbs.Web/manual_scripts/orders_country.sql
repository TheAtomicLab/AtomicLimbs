SELECT O.Id AS 'N°Pedido', U.Country AS Pais, U.Address + ', ' + U.City AS Direccion,
		CASE O.AmputationType
			WHEN 0 THEN 'A'
			WHEN 1 THEN 'B'
			WHEN 2 THEN 'C'
			WHEN 3 THEN 'D'
			WHEN 4 THEN 'E'
			WHEN 5 THEN 'F'
			WHEN 6 THEN 'G'
			WHEN 7 THEN 'H'
		END AS 'Amputación',
		CASE O.Status 
			WHEN 0 THEN 'No asignado'
			WHEN 1 THEN 'Pre-asignado'
			WHEN 2 THEN 'Imprimiendo'
			WHEN 3 THEN 'Lista'
			WHEN 4 THEN 'Entregada'
			WHEN 5 THEN 'Coordinando envío'
			WHEN 6 THEN 'Rechazada'
		END AS Estado
FROM OrderModels O
INNER JOIN UserModels U ON O.OrderRequestor_Id = U.Id
ORDER BY O.Id DESC
