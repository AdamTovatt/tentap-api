﻿CREATE TABLE site_user (
id SERIAL PRIMARY KEY,
name VARCHAR(100),
email VARCHAR(255) UNIQUE NOT NULL,
password VARCHAR(500),
created_date DATE
)