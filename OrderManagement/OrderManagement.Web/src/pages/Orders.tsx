/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from "react";
import { api } from "../api";

type Order = { id: string; title: string; amount: number; status: string; createdAt: string };
type CreateOrderDto = { title: string; amount: number };
type UpdateOrderDto = { title: string; amount: number; status: string };

export default function Orders() {
    const [items, setItems] = useState<Order[]>([]);
    const [title, setTitle] = useState("");
    const [amount, setAmount] = useState<number>(0);
    const [busy, setBusy] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const load = async () => {
        setError(null);
        const { data } = await api.get<Order[]>("/orders");
        setItems(data);
    };

    const create = async () => {
        if (!title) return;
        setBusy(true);
        try {
            const dto: CreateOrderDto = { title, amount };
            await api.post("/orders", dto);
            setTitle(""); setAmount(0);
            await load();
        } catch (e: any) { setError(e.message ?? "create failed"); }
        finally { setBusy(false); }
    };

    const update = async (id: string, status: string) => {
        const o = items.find(x => x.id === id)!;
        setBusy(true);
        try {
            const dto: UpdateOrderDto = { title: o.title, amount: o.amount, status };
            await api.put(`/orders/${id}`, dto);
            await load();
        } catch (e: any) { setError(e.message ?? "update failed"); }
        finally { setBusy(false); }
    };

    const remove = async (id: string) => {
        setBusy(true);
        try { await api.delete(`/orders/${id}`); await load(); }
        catch (e: any) { setError(e.message ?? "delete failed"); }
        finally { setBusy(false); }
    };

    useEffect(() => { load(); }, []);

    return (
        <div style={{ maxWidth: 760, margin: "40px auto", fontFamily: "system-ui, sans-serif" }}>
            <h2>Orders</h2>

            <div style={{ display: "flex", gap: 8, marginBottom: 16 }}>
                <input placeholder="Title" value={title} onChange={e => setTitle(e.target.value)} />
                <input type="number" placeholder="Amount" value={amount} onChange={e => setAmount(+e.target.value)} />
                <button onClick={create} disabled={busy || !title}>Create</button>
            </div>

            {error && <div style={{ color: "crimson", marginBottom: 12 }}>? {error}</div>}

            <table width="100%" cellPadding={6} style={{ borderCollapse: "collapse" }}>
                <thead><tr><th align="left">Title</th><th>Amount</th><th>Status</th><th>Actions</th></tr></thead>
                <tbody>
                    {items.map(o => (
                        <tr key={o.id} style={{ borderTop: "1px solid #ddd" }}>
                            <td>{o.title}</td>
                            <td align="center">{o.amount}</td>
                            <td align="center">{o.status}</td>
                            <td align="right" style={{ display: "flex", gap: 6, justifyContent: "flex-end" }}>
                                <button onClick={() => update(o.id, "in_progress")} disabled={busy}>Start</button>
                                <button onClick={() => update(o.id, "done")} disabled={busy}>Done</button>
                                <button onClick={() => remove(o.id)} disabled={busy}>Delete</button>
                            </td>
                        </tr>
                    ))}
                    {items.length === 0 && (
                        <tr><td colSpan={4} style={{ color: "#888", padding: 12 }}>No orders yet</td></tr>
                    )}
                </tbody>
            </table>
        </div>
    );
}