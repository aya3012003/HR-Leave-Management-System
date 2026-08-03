import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { departmentsApi } from "@/lib/api";
import { Card, CardContent } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { Modal } from "@/components/ui/modal";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Plus } from "lucide-react";

export default function DepartmentsPage() {
  const queryClient = useQueryClient();
  
  const [modalOpen, setModalOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [name, setName] = useState("");

  const { data: depts, isLoading } = useQuery({
    queryKey: ['departments'],
    queryFn: departmentsApi.getAll,
  });

  const saveMutation = useMutation({
    mutationFn: (data: { id: number | null, name: string }) => {
      return data.id 
        ? departmentsApi.update(data.id, { name: data.name })
        : departmentsApi.create({ name: data.name });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['departments'] });
      setModalOpen(false);
    }
  });

  const deleteMutation = useMutation({
    mutationFn: departmentsApi.delete,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['departments'] })
  });

  const openModal = (dept: any = null) => {
    if (dept) {
      setEditingId(dept.id);
      setName(dept.name);
    } else {
      setEditingId(null);
      setName("");
    }
    setModalOpen(true);
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Departments</h1>
          <p className="text-muted-foreground mt-1">Manage organizational structures.</p>
        </div>
        <Button onClick={() => openModal()} className="gap-2">
          <Plus size={16} /> New Department
        </Button>
      </div>

      <Card>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead className="w-full">Department Name</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {isLoading ? (
                <TableRow><TableCell colSpan={3} className="text-center py-8">Loading...</TableCell></TableRow>
              ) : depts && depts.length > 0 ? (
                depts.map(dept => (
                  <TableRow key={dept.id}>
                    <TableCell className="text-muted-foreground">#{dept.id}</TableCell>
                    <TableCell className="font-medium">{dept.name}</TableCell>
                    <TableCell className="text-right space-x-2">
                      <Button variant="ghost" size="sm" onClick={() => openModal(dept)}>Edit</Button>
                      <Button 
                        variant="ghost" 
                        size="sm" 
                        className="text-destructive hover:bg-destructive hover:text-destructive-foreground"
                        onClick={() => {
                          if (confirm(`Delete department ${dept.name}?`)) deleteMutation.mutate(dept.id);
                        }}
                      >
                        Delete
                      </Button>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow><TableCell colSpan={3} className="text-center py-8 text-muted-foreground">No departments found.</TableCell></TableRow>
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <Modal 
        isOpen={modalOpen} 
        onClose={() => setModalOpen(false)} 
        title={editingId ? "Edit Department" : "Create Department"}
      >
        <form onSubmit={e => { e.preventDefault(); saveMutation.mutate({ id: editingId, name }); }} className="space-y-4">
          <div className="space-y-2">
            <Label>Department Name</Label>
            <Input value={name} onChange={e => setName(e.target.value)} required autoFocus />
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
