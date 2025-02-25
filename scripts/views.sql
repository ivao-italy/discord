
CREATE VIEW examlist AS SELECT id, airport, facility, rating, planned_date, planned_hour FROM exam.examList;


CREATE VIEW trainingslist AS
SELECT id, LEFT(airport, 4) as airport, facility, rating_training as rating, planned_date, planned_hour
FROM trainingNEW.trainings;


