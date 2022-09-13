CREATE TABLE course_module (
id SERIAL PRIMARY KEY,
course_id INT REFERENCES course (id),
name VARCHAR(100) NOT NULL
)