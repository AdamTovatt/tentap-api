CREATE TABLE exercise (
id SERIAL PRIMARY KEY,
module_id INT REFERENCES course_module (id),
difficulty INT NOT NULL,
source_id INT REFERENCES source (id),
problem_image VARCHAR(1000),
solution_image VARCHAR(1000),
created_by INT NOT NULL REFERENCES site_user (id),
created_date DATE NOT NULL DEFAULT NOW(),
active BOOLEAN NOT NULL DEFAULT FALSE
)