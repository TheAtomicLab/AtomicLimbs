--AMPUTATION TYPE TABLE
INSERT INTO AmputationTypeModels
VALUES 
	(1, 'A', 'Perdí UNA falange de cualquier dedo', 'https://limbstest.blob.core.windows.net/site/amputationType/a.png', null, 'Sin diseño'),
	(2, 'B', 'Perdí DOS falanges de cualquier dedo', 'https://limbstest.blob.core.windows.net/site/amputationType/b.png', null, 'Sin diseño'),
	(3, 'C', 'Perdí mis cuatro dedos y tengo un pulgar', 'https://limbstest.blob.core.windows.net/site/amputationType/c.png', null, 'Mano con pulgar'),
	(4, 'D', 'Perdí el pulgar y no tengo los dedos. (Poseo hueso carpal)', 'https://limbstest.blob.core.windows.net/site/amputationType/d.png', null, 'Mano'),
	(5, 'E', 'Perdí la mano, no tengo muñeca. (Poseo hueso cúbito y radio pero no carpal)', 'https://limbstest.blob.core.windows.net/site/amputationType/e.png', null, 'Brazo'),
	(6, 'F', 'Tengo un muñón a partir del codo y tengo un antebrazo desarrollado. (Huesos cúbito y radio presentes)', 'https://limbstest.blob.core.windows.net/site/amputationType/f.png', null, 'Brazo'),
	(7, 'G', 'Perdí el codo y tengo el húmero', 'https://limbstest.blob.core.windows.net/site/amputationType/g.png', null, 'Sin diseño'),
	(8, 'H', 'Tengo el húmero pero muy poco desarrollado', 'https://limbstest.blob.core.windows.net/site/amputationType/h.png', null, 'Sin diseño')

INSERT INTO RenderModels
VALUES
	(1, 3, 'PRINTED1', 'https://limbstest.blob.core.windows.net/site/render_hand/PrintedPieces1.png', null),
	(2, 3, 'PRINTED2', 'https://limbstest.blob.core.windows.net/site/render_hand/PrintedPieces2.png', null),
	(3, 3, 'PRINTED3', 'https://limbstest.blob.core.windows.net/site/render_hand/PrintedPieces3.png', null),

	(4, 4, 'PRINTED1', 'https://limbstest.blob.core.windows.net/site/render_hand/PrintedPieces1.png', null),
	(5, 4, 'PRINTED2', 'https://limbstest.blob.core.windows.net/site/render_hand/PrintedPieces2.png', null),
	(6, 4, 'PRINTED3', 'https://limbstest.blob.core.windows.net/site/render_hand/PrintedPieces3.png', null),

	(7, 5, 'PRINTED1', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/arm_PrintedPieces.png', null),
	(8, 5, 'PRINTED2', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/arm_PrintedPieces2.png', null),
	(9, 5, 'PRINTED3', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/arm_PrintedPieces3.png', null),

	(10, 6, 'PRINTED1', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/arm_PrintedPieces.png', null),
	(11, 6, 'PRINTED2', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/arm_PrintedPieces2.png', null),
	(12, 6, 'PRINTED3', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/arm_PrintedPieces3.png', null)

INSERT INTO RenderPieceModels
VALUES 
	(1, 1, 'UpperArm'),
	(2, 1, 'UpperArm-Finger slider'),
	(3, 1, 'FINGERX2normals'),
	(4, 1, 'Thumb connector'),
	(5, 1, 'Thumb'),
	(6, 1, 'FingerX1'),
	(7, 1, 'FINGERS'),
	(8, 4, 'UpperArm'),
	(9, 4, 'UpperArm-Finger slider'),
	(10, 4, 'FINGERX2normals'),
	(11, 4, 'Thumb connector'),
	(12, 4, 'Thumb'),
	(13, 4, 'FingerX1'),
	(14, 4, 'FINGERS'),

	(15, 2, 'Palm'),
	(16, 2, 'AtomicLabCover'),
	(17, 2, 'Fingerstopper'),
	(18, 2, 'FingerMechanismHolder'),
	(19, 5, 'Palm'),
	(20, 5, 'AtomicLabCover'),
	(21, 5, 'Fingerstopper'),
	(22, 5, 'FingerMechanismHolder'),

	(23, 3, 'UpperArm Finger Conector'),
	(24, 3, 'UpperArm Palm connector'),
	(25, 3, 'UpperArm Thumb short connector'),
	(26, 3, 'Thumbscrew'),
	(27, 3, 'ThumbClip'),
	(28, 6, 'UpperArm Finger conector'),
	(29, 6, 'UpperArm Palm connector'),
	(30, 6, 'UpperArm Thumb short connector'),
	(31, 6, 'Thumbscrew'),
	(32, 6, 'ThumbClip'),

	(33, 7, 'Connector W/Screw'),
	(34, 7, 'Thread'),
	(35, 7, 'Connector Bicep Fingers'),
	(36, 7, 'ForearmJoinerB'),
	(37, 7, 'ForearmJoinerA'),
	(38, 7, 'ForearmHolder'),
	(39, 7, 'Right Arm X2'),
	(40, 7, 'AtomicLabCover'),
	(41, 7, 'SmallStumpHolder'),
	(42, 10, 'Connector W/Screw'),
	(43, 10, 'Thread'),
	(44, 10, 'Connector Bicep Fingers'),
	(45, 10, 'ForearmJoinerB'),
	(46, 10, 'ForearmJoinerA'),
	(47, 10, 'ForearmHolder'),
	(48, 10, 'Right Arm X2'),
	(49, 10, 'AtomicLabCover'),
	(50, 10, 'SmallStumpHolder'),

	(51, 8, 'ForearmElbowJoiner'),
	(52, 8, 'UpperArm - Palm connector'),
	(53, 8, 'ThumbClip'),
	(54, 8, 'Thumb'),
	(55, 8, 'FingerMechanismHolder'),
	(56, 8, 'Palm'),
	(57, 8, 'ThumbScrew'),
	(58, 8, 'X1'),
	(59, 8, 'Fingers'),
	(60, 11, 'ForearmElbowJoiner'),
	(61, 11, 'UpperArm - Palm connector'),
	(62, 11, 'ThumbClip'),
	(63, 11, 'Thumb'),
	(64, 11, 'FingerMechanismHolder'),
	(65, 11, 'Palm'),
	(66, 11, 'ThumbScrew'),
	(67, 11, 'X1'),
	(68, 11, 'Fingers'),

	(69, 9, 'ForearmCover'),
	(70, 9, 'BicepHolderB'),
	(71, 9, 'Bicep holder for A and B'),
	(72, 9, 'BicepHolder'),
	(73, 9, 'Upperarm Bicep Holder'),
	(74, 9, 'Clip'),
	(75, 9, 'BicepElbow'),
	(76, 12, 'ForearmCover'),
	(77, 12, 'BicepHolderB'),
	(78, 12, 'Bicep holder for A and B'),
	(79, 12, 'BicepHolder'),
	(80, 12, 'Upperarm Bicep Holder'),
	(81, 12, 'Clip'),
	(82, 12, 'BicepElbow')

INSERT INTO ColorModels
VALUES
	(1, 3, 'A', 'Blanco y rojo', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_a.png', null),
	(2, 3, 'B', 'Azul y rojo', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_b.png', null),
	(3, 3, 'C', 'Rosa y blanco', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_c.png', null),
	(4, 3, 'D', 'Azul y blanco', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_d.png', null),
	(5, 3, 'E', 'Negro y amarillo', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_e.png', null),
	(6, 3, 'F', 'Rojo y amarillo', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_f.png', null),

	(7, 4, 'A', 'Blanco y rojo', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_a.png', null),
	(8, 4, 'B', 'Azul y rojo', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_b.png', null),
	(9, 4, 'C', 'Rosa y blanco', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_c.png', null),
	(10, 4, 'D', 'Azul y blanco', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_d.png', null),
	(11, 4, 'E', 'Negro y amarillo', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_e.png', null),
	(12, 4, 'F', 'Rojo y amarillo', 'https://limbstest.blob.core.windows.net/site/render_hand/colors/mano_f.png', null),

	(13, 5, 'A', 'Blanco y rojo', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_a.png', null),
	(14, 5, 'B', 'Azul y rojo', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_b.png', null),
	(15, 5, 'C', 'Rosa y blanco', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_c.png', null),
	(16, 5, 'D', 'Azul y blanco', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_d.png', null),
	(17, 5, 'E', 'Negro y amarillo', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_e.png', null),
	(18, 5, 'F', 'Rojo y amarillo', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_f.png', null),

	(19, 6, 'A', 'Blanco y rojo', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_a.png', null),
	(20, 6, 'B', 'Azul y rojo', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_b.png', null),
	(21, 6, 'C', 'Rosa y blanco', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_c.png', null),
	(22, 6, 'D', 'Azul y blanco', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_d.png', null),
	(23, 6, 'E', 'Negro y amarillo', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_e.png', null),
	(24, 6, 'F', 'Rojo y amarillo', 'https://limbstest.blob.core.windows.net/site/render_armWithElbow/colors/brazo_f.png', null)

--UPDATE ORDERS NEW FK
UPDATE OrderModels SET ColorFkId = (Color + 1), AmputationTypeFkId = (AmputationType + 1)

--BEGIN TRY
--	BEGIN TRAN nombreTransaccion
--	DECLARE @OrderId int, @AmputationTypeId INT
--	DECLARE @AtomicLabCover bit, @FingerMechanismHolder bit, @Fingers bit,@FingerStopper bit,@FingersX1 bit,@FingersX2P bit,@Palm bit,@Thumb bit,@ThumbClip bit,@ThumbConnector bit,@ThumbScrew bit,@UpperArm bit,@UpperArm_FingerConnector bit,@UpperArm_FingerSlider bit,@UpperArm_PalmConnector bit,@UpperArm_ThumbShortConnector bit
--	DECLARE order_cursor cursor for
--		select o.Id, o.AmputationTypeFkId, o.Pieces_AtomicLabCover, o.Pieces_FingerMechanismHolder, o.Pieces_Fingers, o.Pieces_FingerStopper, o.Pieces_FingersX1, o.Pieces_FingersX2P, o.Pieces_Palm, o.Pieces_Thumb, o.Pieces_ThumbClip, o.Pieces_ThumbConnector, o.Pieces_ThumbScrew, o.Pieces_UpperArm, o.Pieces_UpperArm_FingerConnector, o.Pieces_UpperArm_FingerSlider, o.Pieces_UpperArm_PalmConnector, o.Pieces_UpperArm_ThumbShortConnector 
--		from OrderModels o
--		where o.AmputationTypeFkId = 3 or o.AmputationTypeFkId = 5
--		order by o.AmputationTypeFkId ASC
--	OPEN order_cursor
--	FETCH NEXT FROM order_cursor into @OrderId, @AmputationTypeId, @AtomicLabCover , @FingerMechanismHolder , @Fingers ,@FingerStopper ,@FingersX1 ,@FingersX2P ,@Palm ,@Thumb ,@ThumbClip ,@ThumbConnector ,@ThumbScrew ,@UpperArm ,@UpperArm_FingerConnector ,@UpperArm_FingerSlider ,@UpperArm_PalmConnector, @UpperArm_ThumbShortConnector
--	WHILE @@FETCH_STATUS = 0
--	BEGIN
--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 16, @AtomicLabCover)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 18, @FingerMechanismHolder)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 7, @Fingers)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 17, @FingerStopper)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 6, @FingersX1)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 3, @FingersX2P)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 15, @Palm)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 5, @Thumb)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 27, @ThumbClip)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 4, @ThumbConnector)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 26, @ThumbScrew)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 1, @UpperArm)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 23, @UpperArm_FingerConnector)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 2, @UpperArm_FingerSlider)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 24, @UpperArm_PalmConnector)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 25, @UpperArm_ThumbShortConnector)

--		FETCH NEXT FROM order_cursor into @OrderId, @AmputationTypeId, @AtomicLabCover , @FingerMechanismHolder , @Fingers ,@FingerStopper ,@FingersX1 ,@FingersX2P ,@Palm ,@Thumb ,@ThumbClip ,@ThumbConnector ,@ThumbScrew ,@UpperArm ,@UpperArm_FingerConnector ,@UpperArm_FingerSlider ,@UpperArm_PalmConnector, @UpperArm_ThumbShortConnector
--	END

--	CLOSE order_cursor
--	DEALLOCATE order_cursor

--	COMMIT TRAN nombreTransaccion
--END TRY  
--BEGIN CATCH  
--     ROLLBACK TRAN nombreTransaccion
--END CATCH  

--BEGIN TRY
--	BEGIN TRAN nombreTransaccion
--	DECLARE @OrderId int, @AmputationTypeId INT
--	DECLARE @AtomicLabCover bit, @FingerMechanismHolder bit, @Fingers bit,@FingerStopper bit,@FingersX1 bit,@FingersX2P bit,@Palm bit,@Thumb bit,@ThumbClip bit,@ThumbConnector bit,@ThumbScrew bit,@UpperArm bit,@UpperArm_FingerConnector bit,@UpperArm_FingerSlider bit,@UpperArm_PalmConnector bit,@UpperArm_ThumbShortConnector bit
--	DECLARE order_cursor cursor for
--		select o.Id, o.AmputationTypeFkId, o.Pieces_AtomicLabCover, o.Pieces_FingerMechanismHolder, o.Pieces_Fingers, o.Pieces_FingerStopper, o.Pieces_FingersX1, o.Pieces_FingersX2P, o.Pieces_Palm, o.Pieces_Thumb, o.Pieces_ThumbClip, o.Pieces_ThumbConnector, o.Pieces_ThumbScrew, o.Pieces_UpperArm, o.Pieces_UpperArm_FingerConnector, o.Pieces_UpperArm_FingerSlider, o.Pieces_UpperArm_PalmConnector, o.Pieces_UpperArm_ThumbShortConnector 
--		from OrderModels o
--		where o.AmputationTypeFkId = 4 or o.AmputationTypeFkId = 6
--		order by o.AmputationTypeFkId ASC
--	OPEN order_cursor
--	FETCH NEXT FROM order_cursor into @OrderId, @AmputationTypeId, @AtomicLabCover , @FingerMechanismHolder , @Fingers ,@FingerStopper ,@FingersX1 ,@FingersX2P ,@Palm ,@Thumb ,@ThumbClip ,@ThumbConnector ,@ThumbScrew ,@UpperArm ,@UpperArm_FingerConnector ,@UpperArm_FingerSlider ,@UpperArm_PalmConnector, @UpperArm_ThumbShortConnector
--	WHILE @@FETCH_STATUS = 0
--	BEGIN
--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 20, @AtomicLabCover)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 22, @FingerMechanismHolder)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 14, @Fingers)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 21, @FingerStopper)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 13, @FingersX1)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 10, @FingersX2P)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 19, @Palm)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 12, @Thumb)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 32, @ThumbClip)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 11, @ThumbConnector)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 31, @ThumbScrew)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 8, @UpperArm)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 28, @UpperArm_FingerConnector)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 9, @UpperArm_FingerSlider)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 29, @UpperArm_PalmConnector)

--		INSERT INTO OrderRenderPieceModels 
--		VALUES (@OrderId, 30, @UpperArm_ThumbShortConnector)

--		FETCH NEXT FROM order_cursor into @OrderId, @AmputationTypeId, @AtomicLabCover , @FingerMechanismHolder , @Fingers ,@FingerStopper ,@FingersX1 ,@FingersX2P ,@Palm ,@Thumb ,@ThumbClip ,@ThumbConnector ,@ThumbScrew ,@UpperArm ,@UpperArm_FingerConnector ,@UpperArm_FingerSlider ,@UpperArm_PalmConnector, @UpperArm_ThumbShortConnector
--	END

--	CLOSE order_cursor
--	DEALLOCATE order_cursor

--	COMMIT TRAN nombreTransaccion
--END TRY  
--BEGIN CATCH  
--     ROLLBACK TRAN nombreTransaccion
--END CATCH  

--UPDATE OrderModels SET OrderAmbassador_Id = NULL WHERE AmputationTypeFkId = 1 OR AmputationTypeFkId = 2 OR AmputationTypeFkId = 7 OR AmputationTypeFkId = 8
