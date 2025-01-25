import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { DropdownMenu, DropdownMenuTrigger, DropdownMenuContent, DropdownMenuItem } from "@/components/ui/dropdown-menu";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { GetPartnerContactsByPartnerId } from "@/services/contact";
import { useQuery } from "@tanstack/react-query";
import { Ellipsis, Pencil, SquareX } from "lucide-react";
import { useParams } from "react-router";

const useGetPartnerContactsByPartnerId = (partnerId: string) => {
  return useQuery({
        queryKey: GetPartnerContactsByPartnerId({ partnerId }).key,
        queryFn: GetPartnerContactsByPartnerId({ partnerId }).service,
        select: res => res.data
    });
  }

export const PartnerContacts = () => {
  const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
  const { data: rows } = useGetPartnerContactsByPartnerId(partnerId)
  return (
    <div className="tailwind">
        <div className='container mx-auto'>
            <div className='grid grid-cols-12 !gap-8'>
                <div className='col-span-4'>
                  <Card>
                    <CardHeader>
                        <CardTitle className='font-semibold tracking-tight text-primary text-2xl'>
                            Edit Partner Contacts
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                      This page allows you to add more contacts to this partner so you can share the load of
                      responding to questions for this partner. This information may be displayed in the
                      partnerships page on TrashMob.eco, but is also used by the TrashMob site administrators to
                      contact your organization during setup and during times where issues have arisen.
                    </CardContent>
                </Card>
                </div>
                <div className='col-span-8'>
                  <Card>
                    <CardHeader>
                      <CardTitle className="font-semibold tracking-tight text-primary text-2xl">Partner Contacts</CardTitle>
                    </CardHeader>
                    <CardContent>
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Name</TableHead>
                            <TableHead>Email</TableHead>
                            <TableHead>Phone</TableHead>
                            <TableHead>Actions</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {(rows || []).map(row => (
                            <TableRow key={row.id}>
                              <TableCell>{row.name}</TableCell>
                              <TableCell>{row.email}</TableCell>
                              <TableCell>{row.phone}</TableCell>
                              <TableCell>
                                <DropdownMenu>
                                  <DropdownMenuTrigger asChild>
                                    <Button variant="ghost" size="icon"><Ellipsis /></Button>
                                  </DropdownMenuTrigger>
                                  <DropdownMenuContent className="w-56">
                                    <DropdownMenuItem><Pencil />Edit Contact</DropdownMenuItem>
                                    <DropdownMenuItem><SquareX />Remove Contact</DropdownMenuItem>
                                  </DropdownMenuContent>
                                </DropdownMenu>
                              </TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </CardContent>
                  </Card>
                </div>
            </div>
        </div>
    </div>
  )
}