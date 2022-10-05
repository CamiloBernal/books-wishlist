# **Books Wishlist REST API**

## **A Meli Technical Assessment By Camilo Bernal**

Below, the exercise carried out as part of the technical assessment for Meli is briefly described.

The application was developed always seeking pragmatism, that is, seeking a balance between some good practices and avoiding over-architecture.

Although it is not usual, I tried in this project to make a mixture of different styles (without losing the coherence of the test), to show my knowledge in different techniques and programming styles.

However, to the above, an attempt was made to maintain a framework very close to clean and hexagonal architectures.

The structure of the application is quite simple:

The application is made up of 3 projects:

1. **Application** : To manage the application layer (mainly domain models)
2. **Infrastructure** : To manage the infrastructure layer: Databases, Logs, External Services, etc.
3. **Presentation** : To manage the presentation, in this case, being a REST API, this layer is responsible for managing the REST requests.

The following was the technological stack with which the application was developed:

1. Programming language: C# 10
2. Database: MongoDB
3. Development framework: .Net 6
4. Below: Docker/ Docker compose

To consider, 2 additional tools were included in the development for:

1. Allow to view the Rest API documentation based on OpenAI (Swagger) (Which, in the case of this test, will be in the root of the site).

2. Allow to view the logs and status of the application. (WatchDog, available at: [http://localhost:8080/watchdog](http://localhost:8080/watchdog)

## **Initialize the application:**

In the root path of the project there are the corresponding and sufficient yml files to be able to correctly launch the application with Docker-compose the only thing necessary to be able to execute the application is to execute the command (It is required to have docker installed with docker-compose)
```
Docker-compose up {-d }
```
This command will build the Docker images and perform the necessary actions to have the application mapped to port **_8080_** _http://localhost:8080_. If another port is necessary, the configuration of the Docker compose ymls would have to be modified.

Once the application is up, the endpoints described below will be available.

In case you want to validate the information that is being stored by the application, you can connect a Mongo GUI to port 27017 (raised by Docker compose)

Clarify that the data associated with the passwords of the created users are encrypted in the database. (The encryption mechanism can be seen inside the project: _src/Infrastructure/Services/CryptoService.cs_)

## **End Points**

The following is the list of endpoints that the application exposes:

### **Utilities:**

1. **Swagger (OpenAPI Implementation):** http://localhost:8080/ (Left at this path for demo purposes only)
   ![Swagger](https://resources.camilobernal.dev/images/meli_swagger.png)
2. **Health Check** : http://localhost:8080/health
3. **Log Viewer** : http://localhost:8080/watchdog (**User**:_"MELI"_ **Password**:_"MeliUser1234"_)
   ![watchdog](https://resources.camilobernal.dev/images/meli_watchdog.png)

### **Security**

**User signup** : http://localhost:8080/sign-up

**Authentication token generation** : http://localhost:8080/sign-in

### **Queries to the Google service**

**Query by type** : http://localhost:8080/books/{query\_type}/{term}/q

**Full query** : http://localhost:8080/books/search/q

### **Business operations**

**Wishlist creation** : http://localhost:8080/wishlist **_[POST]_**

**List of WishLists** : http://localhost:8080/wishlist **_[GET]_**

**Delete Wishlist** : http://localhost:8080/wishlist/{List\_Name} **_[DELETE]_**

**Wishlist update** : http://localhost:8080/wishlist/{List\_Name} **_[PUT]_*

**Add books to Wishlist** : http://localhost:8080/wishlist/{List\_Name}/books **_[PUT]_*

**Delete books from Wishlist** : http://localhost:8080/wishlist/{List\_Name}/books/{Book\_ID} **_[DELETE]_**

## **Consult the Google service**

In this regard, there are different query mechanisms to the Google api:

1. Make a typed query, that is, identifying if the query will be made by title, author, publisher, isbn, etc.
 
  ![Query Types](https://resources.camilobernal.dev/images/meli_queryTypes.png)

  ![Query Types 2](https://resources.camilobernal.dev/images/meli_queryTypes2.png)

The images above shows the different possible search alternatives.

In this case, the type of search must be specified in the url including the search terms:
![Search terms](https://resources.camilobernal.dev/images/meli_searchterms.png)

2. You can perform a search with parameters like those exposed by the original google API: basically, passing the query query in the "q" parameter:

![Search terms Q](https://resources.camilobernal.dev/images/meli_searchtermsQ.png)

Additionally, all the queries allow paging and require that the Google APIKey be passed as a parameter in the url (as requested by the test).

---
_Thank MELI for taking the time to review this technical test. I really enjoyed developing it (I hope it shows) and I hope it lives up to your expectations._

![Merge status](https://img.shields.io/github/commit-status/camilobernal/books-wishlist/master/74d98bbdaa52e15b5c007b266cc0dc9fdaa6e07f)

