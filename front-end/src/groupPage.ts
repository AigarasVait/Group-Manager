import React, { useEffect, useState } from "react";
import { apiFetch } from "./api/apiFetch";

interface Member {
  id: number;
  firstName: string;
}

export function MembersList() {
  const [members, setMembers] = useState<Member[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    apiFetch("/members")   // call your backend API endpoint
      .then((res) => res.json())
      .then((data) => {
        setMembers(data);
        setLoading(false);
      })
      .catch((error) => {
        console.error("Failed to fetch members", error);
        setLoading(false);
      });
  }, []);

  if (loading) {
    return React.createElement("div", null, "Loading...");
  }

  return React.createElement(
    "ul",
    null,
    members.map((member) =>
      React.createElement("li", { key: member.id }, member.firstName)
    )
  );
}