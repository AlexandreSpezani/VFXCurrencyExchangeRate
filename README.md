# Currency Exchange Rate Service 💻

Welcome to Currency Exchange Rate Service.

This is an API that allows the management of foreign exchange rates.

## Getting started ✍️

1. Clone the repository.
2. Make sure you have a `MongoDB` instance running (by default on port 27017).
3. Make sure you have a `Kafka` broker running (by default on port 9092).
4. Run.
5. That's it! Up and running on [port 5103](http://localhost:5103/swagger)🚀

## Technology 👨‍💻
- This service is developed in `.Net 8`, using `Kafka` to to send messages and `MongoDB` as storage.

## Improvements ⚒️
- Dockerize the solution.
- Implements protobuf instead `JSON` on `Kafka`, to save some bandwidth.
- The solution is already designed to be expandable, you can easily add another features like new kafka messages and another functionalities besides currency rate. 





