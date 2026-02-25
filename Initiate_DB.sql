CREATE DATABASE IF NOT EXISTS defaultdb;
USE defaultdb;

CREATE TABLE movies (
movie_id INT PRIMARY KEY AUTO_INCREMENT,
movie_name VARCHAR(150) NOT NULL UNIQUE,
genre VARCHAR(50) NOT NULL,
mpa_rating VARCHAR(5) NOT NULL,
run_time SMALLINT
);

CREATE TABLE location_time (
show_id INT PRIMARY KEY AUTO_INCREMENT,
movie_id INT NOT NULL,
theater_room INT NOT NULL,
show_date DATE NOT NULL, 
start_time TIME NOT NULL, 
end_time TIME NOT NULL,
FOREIGN KEY (movie_id) REFERENCES movies(movie_id)
);

CREATE TABLE employees(
employee_id INT PRIMARY KEY AUTO_INCREMENT, 
employee_start_title VARCHAR(50) NOT NULL, 
employee_current_title VARCHAR(50) NOT NULL,
employee_start_date DATE NOT NULL, 
employee_end_date DATE, 
employee_dob DATE NOT NULL, 
employee_phone SMALLINT NOT NULL,
employee_email VARCHAR (100) NOT NULL,
employee_st_address VARCHAR (100),
employee_city VARCHAR (50) NOT NULL,
employee_state VARCHAR (40) NOT NULL,
employee_zipcode SMALLINT NOT NULL
);



