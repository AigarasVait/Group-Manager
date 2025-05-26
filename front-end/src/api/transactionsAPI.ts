import { API_TRANSACTIONS_ENDPOINT } from "../constants/apiEndpoints";
import { apiFetch } from "./apiFetch";
import type { TransactionCreateDto, TransactionDto } from "../types/Transaction";

export async function postTransaction(transaction: TransactionCreateDto): Promise<TransactionDto> {
    try {
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
        return await response.json();
    } catch (error) {
        console.error("Failed to fetch groups:", error);
        throw error;
    }
}