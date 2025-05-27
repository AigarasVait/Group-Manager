import { useState } from "react";
import type { TransactionCreateDto } from "../../types/Transaction";
import type { GroupDto } from "../../types/Group";
import "./NewTransactionForm.css";

interface NewGroupFormProps {
  onCreate: (newTransaction: TransactionCreateDto) => void;
  onCancel: () => void;
  group?: GroupDto;
  from: number | null;
}

export default function NewGroupForm({
  onCreate,
  onCancel,
  group,
  from
}: NewGroupFormProps) {

  const [amountIn, setAmountIn] = useState(0);
  const membersCount = group?.members.length || 0;
  const [values, setValues] = useState<number[]>(Array(membersCount).fill(0));
  const [descriptionItem, setDescriptionItem] = useState("");
  const [splitTypeItem, setSplitTypeItem] = useState<0 | 1 | 2 | null>(null); // 0 - equal, 1 - dynamic, 2 - percentage

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (splitTypeItem === null) {
      alert("Please select a split type.");
      return;
    }

    const total = values.reduce((sum, val) => sum + val, 0);

    // For percentage split, values must add up to 100
    if (splitTypeItem === 2 && total !== 100) {
      alert("Numbers in percentage split must sum to 100.");
      return;
    }

    // For dynamic split, values must match the total amount
    if (splitTypeItem === 1 && total !== amountIn) {
      alert("Numbers in dynamic split must sum to the total amount.");
      return;
    }
     
    console.error(from);

    const newTransaction: TransactionCreateDto = {
      groupId: group?.id || null,
      payerId: from,
      amount: amountIn,
      description: descriptionItem,
      sType: splitTypeItem,
      splitValues: values,
    };

    onCreate(newTransaction);

    // Reset form
    setDescriptionItem("");
    setSplitTypeItem(null);
    setAmountIn(0);
  };

  return (
    <div className="form-container">
      <form
        onSubmit={handleSubmit}
        className="border border-grey p-3 rounded bg-light py-4 px-4"
      >
        <p className="h4">New payment</p>

        {/* Description Field */}
        <div className="mb-3">
          <label htmlFor="description" className="form-label">
            Description
          </label>
          <input
            type="text"
            id="description"
            className="form-control"
            value={descriptionItem}
            onChange={(e) => setDescriptionItem(e.target.value)}
          />
        </div>

        {/* Amount Field */}
        <div className="mb-3">
          <label htmlFor="amount" className="form-label">
            Amount
          </label>
          <input
            type="number"
            step="0.01"
            id="amount"
            className="form-control"
            required
            value={amountIn}
            onChange={(e) => setAmountIn(Number(e.target.value))}
          />
        </div>

        {/* Split Type Selection */}
        <div className="mb-3">
          <div className="form-check">
            <input
              type="checkbox"
              id="equalSplit"
              className="form-check-input"
              checked={splitTypeItem === 0}
              onChange={() => setSplitTypeItem(0)}
            />
            <label htmlFor="equalSplit" className="form-check-label">
              Equal split
            </label>
          </div>

          <div className="form-check">
            <input
              type="checkbox"
              id="dynamicSplit"
              className="form-check-input"
              checked={splitTypeItem === 1}
              onChange={() => setSplitTypeItem(1)}
            />
            <label htmlFor="dynamicSplit" className="form-check-label">
              Dynamic
            </label>
          </div>

          <div className="form-check">
            <input
              type="checkbox"
              id="percentageSplit"
              className="form-check-input"
              checked={splitTypeItem === 2}
              onChange={() => setSplitTypeItem(2)}
            />
            <label htmlFor="percentageSplit" className="form-check-label">
              Percentage
            </label>
          </div>
        </div>

        {/* Input Fields for Dynamic or Percentage Splits */}
        {(splitTypeItem === 1 || splitTypeItem === 2) && (
          <div className="mb-3">
            {values.map((val, index) => (
              <div key={index}>
                <label className="form-label">
                  {group?.members[index]?.name}
                </label>
                <input
                  type="number"
                  step="0.01"
                  required
                  value={val}
                  onChange={(e) => {
                    const updated = [...values];
                    updated[index] = Number(e.target.value);
                    setValues(updated);
                  }}
                  className="form-control mb-2"
                />
              </div>
            ))}
          </div>
        )}

        {/* Form Buttons */}
        <div>
          <button type="submit" className="btn btn-primary">
            Create
          </button>
          <button
            type="button"
            onClick={onCancel}
            className="btn btn-danger ms-3"
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
