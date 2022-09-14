CREATE TABLE user_completed_exercise (
user_id INT NOT NULL REFERENCES site_user (id),
exercise_id INT NOT NULL REFERENCES exercise (id),
PRIMARY KEY (user_id, exercise_id)
)