CREATE TABLE user_completed_exercise (
user_id INT NOT NULL REFERENCES site_user (id),
exercise_id INT NOT NULL REFERENCES exercise (id),
perceived_difficulty INT NOT NULL,
completed_time TIMESTAMP DEFAULT NOW(),
PRIMARY KEY (user_id, exercise_id)
)