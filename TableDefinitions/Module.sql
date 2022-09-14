CREATE TABLE course_module (
id SERIAL PRIMARY KEY,
course_id INT REFERENCES course (id),
name VARCHAR(100) NOT NULL,
created_by INT NOT NULL REFERENCES site_user (id)
)