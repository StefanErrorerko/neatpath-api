# NeatPath API
## Project Overview
NeatPath is a URL shortening application built with React.js for the frontend and ASP.NET for the backend API. DB is located on MsSql.

## About API
API implements ASP.NET Core MVC pattern, follows REST API principles uses hashing and crypto algorithms inside. 
1. Password stored in DB as a hash. Hash is creating by BCrypt algorithm. It keeps user privacy information in safety.
2. Short Url hashes are creating with MD5 and Base62 Encoding. Written by an article: https://blog.algomaster.io/p/design-a-url-shortener
- MD5 Cryptographic Hashing: A URL starts processing by creating a unique digital fingerprint of your URL using the MD5 hashing algorithm, ensuring consistent and secure transformation
- Efficient Compression: The system intelligently selects 6 bytes from the hash, compressing the fingerprint while maintaining uniqueness
- Base62 Encoding:Then it converts the compressed hash into a readable format using numbers (0-9), lowercase (a-z), and uppercase (A-Z) letters, creating short, user-friendly URLs
- Deterministic Processing: Finally, algorithm ensures that the same input URL always produces the same shortened link, preventing duplicates and maintaining consistency

## UML 
![Діаграма без назви drawio](https://github.com/user-attachments/assets/7a922e25-558b-44e0-b283-9a0fd2e4f768)

## Screenshots
### API UI
![image](https://github.com/user-attachments/assets/a195cd7f-47e6-4871-a80c-2d435d604d16)

### Class Diagram
![image](https://github.com/user-attachments/assets/e5cc267b-e7c8-4a96-b009-9cb7e9ee7708)
![image](https://github.com/user-attachments/assets/3a0db000-536f-4081-9af5-732c31fbeffc)

## URL Management
- Shorten long URLs to compact, manageable links
- View, add, and delete URL shortcuts
- Role-based access control for URL operations

## Backen Architecture
Framework: ASP.NET MVC
SwaggerUi, REST API

Stefan Muzyka
2025
