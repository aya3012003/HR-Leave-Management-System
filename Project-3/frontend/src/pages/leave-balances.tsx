import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { leaveBalancesApi, employeesApi } from "@/lib/api";
import { useAuth } from "@/contexts/AuthContext";
import { Card, CardContent } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { Modal } from "@/components/ui/modal";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

export default function LeaveBalancesPage() {
  const { isAdmin } = useAuth();
  const queryClient = useQueryClient();
  const [page, setPage] = useState(1);
  
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [selectedBalance, setSelectedBalance] = useState<any>(null);
  const [remainingDays, setRemainingDays] = useState("");

  const { data: pageData, isLoading } = useQuery({
    queryKey: ['all-leave-balances', page],
    queryFn: () => leaveBalancesApi.getAll({ page, pageSize: 20 }),
  });

  const updateMutation = useMutation({
    mutationFn: (data: { id: number, remainingDays: number }) => 
      leaveBalancesApi.update(data.id, { remainingDays: data.remainingDays }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['all-leave-balances'] });
      setEditModalOpen(false);
    }
  });

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Leave Balances</h1>
        <p className="text-muted-foreground mt-1">View and manage staff leave quotas.</p>
      </div>

      <Card>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Employee</TableHead>
                <TableHead>Leave Type</TableHead>
                <TableHead className="text-right">Remaining Days</TableHead>
                {isAdmin && <TableHead className="text-right">Actions</TableHead>}
              </TableRow>
            </TableHeader>
            <TableBody>
              {isLoading ? (
                <TableRow><TableCell colSpan={4} className="text-center py-8">Loading...</TableCell></TableRow>
              ) : pageData?.items && pageData.items.length > 0 ? (
                pageData.items.map(bal => (
                  <TableRow key={bal.id}>
                    <TableCell className="font-medium">{bal.employeeName}</TableCell>
                    <TableCell>{bal.leaveTypeName}</TableCell>
                    <TableCell className="text-right font-bold">{bal.remainingDays}</TableCell>
                    {isAdmin && (
                      <TableCell className="text-right">
                        <Button 
                          variant="ghost" 
                          size="sm" 
                          onClick={() => {
                            setSelectedBalance(bal);
                            setRemainingDays(String(bal.remainingDays));
                            setEditModalOpen(true);
                          }}
                        >
                          Edit
                        </Button>
                      </TableCell>
                    )}
                  </TableRow>
                ))
              ) : (
                <TableRow><TableCell colSpan={4} className="text-center py-8 text-muted-foreground">No balances found.</TableCell></TableRow>
              )}
            </TableBody>
          </Table>
          
          {pageData && pageData.totalPages > 1 && (
            <div className="flex items-center justify-between px-6 py-3 border-t">
              <span className="text-sm text-muted-foreground">
                Page {pageData.pageNumber} of {pageData.totalPages}
              </span>
              <div className="space-x-2">
                <Button variant="outline" size="sm" disabled={page === 1} onClick={() => setPage(p => p - 1)}>Previous</Button>
                <Button variant="outline" size="sm" disabled={page === pageData.totalPages} onClick={() => setPage(p => p + 1)}>Next</Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      <Modal 
        isOpen={editModalOpen} 
        onClose={() => setEditModalOpen(false)} 
        title="Adjust Leave Balance"
      >
        <div className="space-y-4">
          <p className="text-sm text-muted-foreground">
            Adjusting balance for <strong>{selectedBalance?.employeeName}</strong> ({selectedBalance?.leaveTypeName}).
          </p>
          <div className="space-y-2">
            <Label>Remaining Days</Label>
            <Input 
              type="number" 
              value={remainingDays} 
              onChange={e => setRemainingDays(e.target.value)} 
              min={0}
              step={0.5}
            />
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <Button variant="outline" onClick={() => setEditModalOpen(false)}>Cancel</Button>
            <Button 
              onClick={() => updateMutation.mutate({ id: selectedBalance.id, remainingDays: Number(remainingDays) })}
              disabled={updateMutation.isPending}
            >
              {updateMutation.isPending ? "Saving..." : "Save Balance"}
            </Button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
