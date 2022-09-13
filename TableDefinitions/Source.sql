CREATE TABLE source (
id SERIAL PRIMARY KEY,
course_id INT REFERENCES course (id),
author VARCHAR(500),
source_date DATE
)