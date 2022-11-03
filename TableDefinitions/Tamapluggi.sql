CREATE TABLE tamapluggi (
id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
user_id INT REFERENCES site_user(id),
name VARCHAR(100) NOT NULL,
study_goal INT NOT NULL,
break_duration INT NOT NULL
)