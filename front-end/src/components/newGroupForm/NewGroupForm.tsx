import React, { use } from "react";
import { useState } from "react";

export default function NewGroupForm() {
    const [open, setOpen] = useState(false);
    const [groupName, setGroupName] = useState("");

    return (
        <div>
            <button
                onClick={(() => setOpen(!open))}
                className="btn btn-primary">
                {open ? "Close" : "Add new"}
            </button>

            {open && (
                <div className="form-container">
                    <form >
                        <div className="mb-3">
                            <label htmlFor="groupName"  className="form-label">Group name</label>
                            <input onChange={(e) => setGroupName(e.target.value)} type="text" className="form-control" id="groupName" />
                        </div>
                        <button type="submit" className="btn btn-primary">Create</button>
                    </form>
                </div>
            )}
        </div>

    );
}