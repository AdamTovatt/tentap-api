CREATE TABLE exercise (
id SERIAL PRIMARY KEY,
module_id INT REFERENCES course_module (id),
difficulty INT NOT NULL,
source_id INT REFERENCES source (id),
problem_image VARCHAR(1000) NOT NULL,
solution_image VARCHAR(1000) NOT NULL,
created_by INT NOT NULL REFERENCES site_user (id)
)