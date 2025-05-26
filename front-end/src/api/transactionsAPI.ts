import { API_TRANSACTIONS_ENDPOINT } from "../constants/apiEndpoints";
import { apiFetch } from "./apiFetch";
import type { TransactionCreateDto, TransactionDto } from "../types/Transaction";

/**
 * Posts a new transaction to the server.
 * Throws an error if the request fails.
 */

export async function postTransaction(transaction: TransactionCreateDto): Promise<TransactionDto> {
    const response = await apiFetch(API_TRANSACTIONS_ENDPOINT, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(transaction)
    });

    if (!response.ok) {
        const text = await response.text();
        console.error("Server said:", text);
        throw new Error(text);
    }

    return response.json();
}
