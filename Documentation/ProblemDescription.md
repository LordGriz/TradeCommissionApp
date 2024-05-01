# Commission Calculation Service

## Assignment
Design and implement a Trade Commission calculation service. This service will accept data for multiple
financial trades and return the total commission amount calculated for all the specified trades.

The data related to the trades passed in to the service is:
* Timestamp of the trade
* Security Type of the traded security (COM, CB or FX)
* Transaction Type (BUY or SELL)
* Quantity of the traded security
* Price at which the trade was executed

From this data, the trade’s amount can be calculated as (Quantity * Price).


## Commission Calculation
The commission for a given trade can be calculated as described in the below table:

| **Security type** | **Transaction Type** | **Commission Amount**                                                                                |
|-------------------|----------------------|------------------------------------------------------------------------------------------------------|
| COM               | BUY                  | 0.05% of the trade’s amount                                                                          |
| COM               | SELL                 | 0.05 % of the trade’s Amount plus an advisory fee of $500 if the Amount is greater than $100,000.00  |
| CB                | BUY                  | 0.02% of the trade’s Amount                                                                          |
| CB                | SELL                 | 0.01 % of the trade’s Amount                                                                         |
| FX                | BUY                  | 0.01% of the trade’s Amount                                                                          |
| FX                | SELL                 | $100 if the Amount is greater than $10,000.00, and $1000 if the Amount is greater than $1,000,000.00 |

## Example 

| Timestamp        | 01/01/2019 10:00:00 |
|------------------|---------------------|
| Security Type    | COM                 |
| Transaction Type | BUY                 |
| Quantity         | 1000                |
| Price            | 12                  |


Commission = (12*1000) *(0.05/100) = $6

## Extra Credit
Design your service to have the ability to calculate the commission for the specified trades in parallel
with a maximum of 10 trades processed at a time.
