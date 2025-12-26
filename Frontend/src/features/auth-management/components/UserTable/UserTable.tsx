import { useState, useMemo } from 'react';
import {
  useReactTable,
  getCoreRowModel,
  getFilteredRowModel,
  getSortedRowModel,
  flexRender,
  type ColumnDef,
  type SortingState,
} from '@tanstack/react-table';
import { Search, ChevronUp, ChevronDown, Edit2 } from 'lucide-react';
import { Badge } from '@/components/ui';
import type { UserDto } from '../../types';
import styles from './UserTable.module.css';

interface UserTableProps {
  users: UserDto[];
  onUserSelect: (user: UserDto) => void;
}

export default function UserTable({ users, onUserSelect }: UserTableProps) {
  const [globalFilter, setGlobalFilter] = useState('');
  const [sorting, setSorting] = useState<SortingState>([]);

  const columns = useMemo<ColumnDef<UserDto>[]>(
    () => [
      {
        accessorKey: 'firstName',
        header: 'Name',
        cell: ({ row }) => (
          <div className={styles.nameCell}>
            <span className={styles.name}>
              {row.original.firstName} {row.original.lastName}
            </span>
          </div>
        ),
      },
      {
        accessorKey: 'email',
        header: 'Email',
        cell: ({ row }) => <span className={styles.email}>{row.original.email}</span>,
      },
      {
        id: 'roles',
        header: 'Assigned Roles',
        accessorFn: (row) => row.roles.map((r) => r.name).join(', '),
        cell: ({ row }) => (
          <div className={styles.rolesCell}>
            {row.original.roles.length === 0 ? (
              <span className={styles.noRoles}>No roles</span>
            ) : (
              row.original.roles.map((role) => (
                <Badge key={role.id} variant="default">
                  {role.name}
                </Badge>
              ))
            )}
          </div>
        ),
      },
      {
        id: 'actions',
        header: 'Actions',
        cell: ({ row }) => (
          <button
            className={styles.actionButton}
            onClick={() => onUserSelect(row.original)}
            title="Edit user"
          >
            <Edit2 size={16} />
            Edit
          </button>
        ),
      },
    ],
    [onUserSelect]
  );

  const table = useReactTable({
    data: users,
    columns,
    state: {
      globalFilter,
      sorting,
    },
    onGlobalFilterChange: setGlobalFilter,
    onSortingChange: setSorting,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getSortedRowModel: getSortedRowModel(),
  });

  return (
    <div className={styles.container}>
      <div className={styles.searchContainer}>
        <Search size={20} className={styles.searchIcon} />
        <input
          type="text"
          placeholder="Search users by name, email, or role..."
          value={globalFilter}
          onChange={(e) => setGlobalFilter(e.target.value)}
          className={styles.searchInput}
        />
      </div>

      <div className={styles.tableWrapper}>
        <table className={styles.table}>
          <thead className={styles.thead}>
            {table.getHeaderGroups().map((headerGroup) => (
              <tr key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <th
                    key={header.id}
                    className={styles.th}
                    onClick={header.column.getToggleSortingHandler()}
                  >
                    <div className={styles.headerContent}>
                      {flexRender(header.column.columnDef.header, header.getContext())}
                      {header.column.getCanSort() && (
                        <span className={styles.sortIcon}>
                          {header.column.getIsSorted() === 'asc' ? (
                            <ChevronUp size={16} />
                          ) : header.column.getIsSorted() === 'desc' ? (
                            <ChevronDown size={16} />
                          ) : (
                            <span className={styles.sortPlaceholder}>⇅</span>
                          )}
                        </span>
                      )}
                    </div>
                  </th>
                ))}
              </tr>
            ))}
          </thead>
          <tbody className={styles.tbody}>
            {table.getRowModel().rows.length === 0 ? (
              <tr>
                <td colSpan={columns.length} className={styles.emptyState}>
                  <div className={styles.emptyContent}>
                    <p>No users found</p>
                    {globalFilter && (
                      <button className={styles.clearButton} onClick={() => setGlobalFilter('')}>
                        Clear search
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ) : (
              table.getRowModel().rows.map((row) => (
                <tr key={row.id} className={styles.row}>
                  {row.getVisibleCells().map((cell) => (
                    <td key={cell.id} className={styles.td}>
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
