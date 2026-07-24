import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { leaveTypesApi } from "@/lib/api";
import { Card, CardContent } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { Modal } from "@/components/ui/modal";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Plus } from "lucide-react";

export default function LeaveTypesPage() {
  const queryClient = useQueryClient();
  const [page, setPage] = useState(1);
  
  const [modalOpen, setModalOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [formData, setFormData] = useState({ name: "", defaultDays: "", description: "" });

  const { data: pageData, isLoading } = useQuery({
    queryKey: ['leave-types', page],
    queryFn: () => leaveTypesApi.getAll({ page, pageSize: 20 }),
  });

  const saveMutation = useMutation({
    mutationFn: (data: { id: number | null, payload: any }) => {
      return data.id 
        ? leaveTypesApi.update(data.id, data.payload)
        : leaveTypesApi.create(data.payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['leave-types'] });
      setModalOpen(false);
    }
  });

  const deleteMutation = useMutation({
    mutationFn: leaveTypesApi.delete,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['leave-types'] })
  });

  const openModal = (lt: any = null) => {
    if (lt) {
      setEditingId(lt.id);
      setFormData({ name: lt.name, defaultDays: String(lt.defaultDays), description: lt.description || "" });
    } else {
      setEditingId(null);
      setFormData({ name: "", defaultDays: "", description: "" });
    }
    setModalOpen(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    saveMutation.mutate({ 
      id: editingId, 
      payload: { 
        name: formData.name, 
        defaultDays: Number(formData.defaultDays),
        description: formData.description 
      }
    });
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Leave Types</h1>
          <p className="text-muted-foreground mt-1">Configure company leave policies.</p>
        </div>
        <Button onClick={() => openModal()} className="gap-2">
          <Plus size={16} /> New Leave Type
        </Button>
      </div>

      <Card>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Name</TableHead>
                <TableHead>Default Days/Year</TableHead>
                <TableHead className="w-[40%]">Description</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {isLoading ? (
                <TableRow><TableCell colSpan={4} className="text-center py-8">Loading...</TableCell></TableRow>
              ) : pageData?.items && pageData.items.length > 0 ? (
                pageData.items.map(lt => (
                  <TableRow key={lt.id}>
                    <TableCell className="font-medium">{lt.name}</TableCell>
                    <TableCell>{lt.defaultDays}</TableCell>
                    <TableCell className="text-muted-foreground truncate max-w-[200px]">{lt.description}</TableCell>
                    <TableCell className="text-right space-x-2">
                      <Button variant="ghost" size="sm" onClick={() => openModal(lt)}>Edit</Button>
                      <Button 
                        variant="ghost" 
                        size="sm" 
                        className="text-destructive hover:bg-destructive hover:text-destructive-foreground"
                        onClick={() => {
                          if (confirm(`Delete leave type ${lt.name}?`)) deleteMutation.mutate(lt.id);
                        }}
                      >
                        Delete
                      </Button>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow><TableCell colSpan={4} className="text-center py-8 text-muted-foreground">No leave types found.</TableCell></TableRow>
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <Modal 
        isOpen={modalOpen} 
        onClose={() => setModalOpen(false)} 
        title={editingId ? "Edit Leave Type" : "Create Leave Type"}
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-3 gap-4">
            <div className="space-y-2 col-span-2">
              <Label>Name *</Label>
              <Input value={formData.name} onChange={e => setFormData({...formData, name: e.target.value})} required autoFocus />
            </div>
            <div className="space-y-2">
              <Label>Default Days *</Label>
              <Input type="number" min="0" step="0.5" value={formData.defaultDays} onChange={e => setFormData({...formData, defaultDays: e.target.value})} required />
            </div>
          </div>
          <div className="space-y-2">
            <Label>Description</Label>
            <Textarea value={formData.description} onChange={e => setFormData({...formData, description: e.target.value})} />
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <Button type="button" variant="outline" onClick={() => setModalOpen(false)}>Cancel</Button>
            <Button type="submit" disabled={saveMutation.isPending}>
              {saveMutation.isPending ? "Saving..." : "Save"}
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
