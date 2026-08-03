import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { leaveRequestsApi } from "@/lib/api";
import { Card, CardContent } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { Modal } from "@/components/ui/modal";
import { Textarea } from "@/components/ui/textarea";
import { SelectNative } from "@/components/ui/select-native";
import { statusColor, formatDate } from "@/lib/utils";

export default function LeaveRequestsPage() {
  const queryClient = useQueryClient();
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState("");
  
  // Action Modal State
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedReq, setSelectedReq] = useState<any>(null);
  const [actionType, setActionType] = useState<"Approve" | "Reject">("Approve");
  const [comment, setComment] = useState("");

  const { data: pageData, isLoading } = useQuery({
    queryKey: ['leave-requests', 'all', page, statusFilter],
    queryFn: () => leaveRequestsApi.getAll({ page, pageSize: 10, status: statusFilter || undefined }),
  });

  const actionMutation = useMutation({
    mutationFn: (data: { id: number, type: "Approve" | "Reject", comment: string }) => {
      const dto = { managerComment: data.comment };
      return data.type === "Approve" ? leaveRequestsApi.approve(data.id, dto) : leaveRequestsApi.reject(data.id, dto);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['leave-requests'] });
      setModalOpen(false);
    }
  });

  const openModal = (req: any, type: "Approve" | "Reject") => {
    setSelectedReq(req);
    setActionType(type);
    setComment("");
    setModalOpen(true);
  };

  const handleAction = () => {
    if (selectedReq) {
      actionMutation.mutate({ id: selectedReq.id, type: actionType, comment });
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Team Leave Requests</h1>
          <p className="text-muted-foreground mt-1">Review and process leave applications.</p>
        </div>
        <div className="w-48">
          <SelectNative value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}>
            <option value="">All Statuses</option>
            <option value="Pending">Pending</option>
            <option value="Approved">Approved</option>
            <option value="Rejected">Rejected</option>
            <option value="Cancelled">Cancelled</option>
          </SelectNative>
        </div>
      </div>

      <Card>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Employee</TableHead>
                <TableHead>Type</TableHead>
                <TableHead>Dates</TableHead>
                <TableHead>Days</TableHead>
                <TableHead>Status</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {isLoading ? (
                <TableRow><TableCell colSpan={6} className="text-center py-8">Loading...</TableCell></TableRow>
              ) : pageData?.items && pageData.items.length > 0 ? (
                pageData.items.map(req => (
                  <TableRow key={req.id}>
                    <TableCell className="font-medium">{req.employeeName}</TableCell>
                    <TableCell>{req.leaveTypeName}</TableCell>
                    <TableCell className="text-muted-foreground text-sm">
                      {formatDate(req.startDate)} - {formatDate(req.endDate)}
                    </TableCell>
                    <TableCell>{req.workingDays}</TableCell>
                    <TableCell>
                      <span className={`px-2.5 py-1 rounded-full text-xs font-medium ${statusColor(req.status)}`}>
                        {req.status}
                      </span>
                    </TableCell>
                    <TableCell className="text-right space-x-2">
                      {req.status === 'Pending' && (
                        <>
                          <Button variant="outline" size="sm" onClick={() => openModal(req, "Reject")}>Reject</Button>
                          <Button size="sm" onClick={() => openModal(req, "Approve")}>Approve</Button>
                        </>
                      )}
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow><TableCell colSpan={6} className="text-center py-8 text-muted-foreground">No leave requests found.</TableCell></TableRow>
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
        isOpen={modalOpen} 
        onClose={() => setModalOpen(false)} 
        title={`${actionType} Leave Request`}
        description={`Add a comment for ${selectedReq?.employeeName}'s ${selectedReq?.leaveTypeName} request.`}
      >
        <div className="space-y-4">
          <Textarea 
            placeholder="Optional comment..." 
            value={comment} 
            onChange={(e) => setComment(e.target.value)}
          />
          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={() => setModalOpen(false)}>Cancel</Button>
            <Button 
              variant={actionType === "Reject" ? "destructive" : "default"} 
              onClick={handleAction}
              disabled={actionMutation.isPending}
            >
              {actionMutation.isPending ? "Saving..." : `Confirm ${actionType}`}
            </Button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
