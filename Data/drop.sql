-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2021-04-05 12:56:53.13

-- foreign keys
ALTER TABLE Product_Warehouse DROP CONSTRAINT Product_Warehouse_Order;

ALTER TABLE "Order" DROP CONSTRAINT Receipt_Product;

ALTER TABLE Product_Warehouse DROP CONSTRAINT _Product;

ALTER TABLE Product_Warehouse DROP CONSTRAINT _Warehouse;

-- tables
DROP TABLE "Order";

DROP TABLE Product;

DROP TABLE Product_Warehouse;

DROP TABLE Warehouse;

-- End of file.