﻿CREATE TABLE course (
id SERIAL PRIMARY KEY,
code VARCHAR(50) UNIQUE NOT NULL,
name VARCHAR(100) NOT NULL,
created_by INT NOT NULL REFERENCES site_user (id)
)