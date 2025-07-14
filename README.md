# MediFlow

**MediFlow** is a hospital workflow management system focusing on vaccination and medical examination processes. It is designed with a microservices architecture to ensure scalability, maintainability, and seamless integration with existing healthcare systems. This project serves as the graduation thesis for a team of software engineering students.

## 🌐 Technologies Used

- **Frontend**: ReactJS + TypeScript + SCSS
- **Backend**: ASP.NET Core Web API (Microservices)
- **Database**: PostgreSQL
- **Caching**: Redis
- **Message Queue**: RabbitMQ
- **Authentication**: ASP.NET Core Identity + JWT
- **CI/CD**: GitHub Actions
- **Deployment**: AWS, Vercel, VPS
- **Monitoring & Logging**: Grafana, Prometheus, ELK Stack

## 🧩 System Architecture

- **Mono-repo**: Modular project structure with individual services containerized using Docker
- **Kubernetes**: Used for orchestrating and managing containers
- **gRPC & REST**: Combined usage for high-performance inter-service communication

## 📦 Project Modules

- User & Identity Service  
- Patient & Medical Records Service
- Appointment & Treatment Service
- Inventory & Vaccine Management Service
- Notification Service
- Analytics & Reporting Service
- Admin & System Management
