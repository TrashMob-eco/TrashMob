import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Container, Dropdown, Form, OverlayTrigger, Row, ToggleButton, Tooltip } from 'react-bootstrap';
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import { Guid } from 'guid-typescript';
import SocialMediaAccountTypeData from '../Models/SocialMediaAccountTypeData';
import PartnerSocialMediaAccountData from '../Models/PartnerSocialMediaAccountData';
import { getSocialMediaAccountType } from '../../store/socialMediaAccountTypeHelper';
import { Pencil, XSquare } from 'react-bootstrap-icons';
import { useMutation, useQuery } from '@tanstack/react-query';
import { CreatePartnerSocialMediaAccount, DeletePartnerSocialMediaAccountById, GetPartnerSocialMediaAccount, GetPartnerSocialMediaAccountsByPartnerId, GetSocialMediaAccountTypes, UpdatePartnerSocialMediaAccount } from '../../services/social-media';
import { Services } from '../../config/services.config';

export interface PartnerSocialMediaAccountsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerSocialMediaAccounts: React.FC<PartnerSocialMediaAccountsDataProps> = (props) => {

    const [socialMediaAccountId, setSocialMediaAccountId] = React.useState<string>(Guid.EMPTY);
    const [accountName, setAccountName] = React.useState<string>("");
    const [isActive, setIsActive] = React.useState<boolean>(true);
    const [socialMediaAccounts, setSocialMediaAccounts] = React.useState<PartnerSocialMediaAccountData[]>([]);
    const [isSocialMediaAccountsDataLoaded, setIsSocialMediaAccountDataLoaded] = React.useState<boolean>(false);
    const [socialMediaAccountTypeId, setSocialMediaAccountTypeId] = React.useState<number>(0);
    const [accountNameErrors, setAccountNameErrors] = React.useState<string>("");
    const [socialMediaAccountTypeList, setSocialMediaAccountTypeList] = React.useState<SocialMediaAccountTypeData[]>([]);
    const [isEditOrAdd, setIsEditOrAdd] = React.useState<boolean>(false);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>(Guid.EMPTY);
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [isAddEnabled, setIsAddEnabled] = React.useState<boolean>(true);

    const getSocialMediaAccountTypes = useQuery({
        queryKey: GetSocialMediaAccountTypes().key,
        queryFn: GetSocialMediaAccountTypes().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false
    });

    const getPartnerSocialMediaAccountsByPartnerId = useQuery({
        queryKey: GetPartnerSocialMediaAccountsByPartnerId({ partnerId: props.partnerId }).key,
        queryFn: GetPartnerSocialMediaAccountsByPartnerId({ partnerId: props.partnerId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false
    });

    const getPartnerSocialMediaAccount = useMutation({
        mutationKey: GetPartnerSocialMediaAccount().key,
        mutationFn: GetPartnerSocialMediaAccount().service
    })

    const createPartnerSocialMediaAccount = useMutation({
        mutationKey: CreatePartnerSocialMediaAccount().key,
        mutationFn: CreatePartnerSocialMediaAccount().service
    })

    const updatePartnerSocialMediaAccount = useMutation({
        mutationKey: UpdatePartnerSocialMediaAccount().key,
        mutationFn: UpdatePartnerSocialMediaAccount().service
    })

    const deletePartnerSocialMediaAccountById = useMutation({
        mutationKey: DeletePartnerSocialMediaAccountById().key,
        mutationFn: DeletePartnerSocialMediaAccountById().service
    })

    React.useEffect(() => {
        getSocialMediaAccountTypes.refetch().then(res => {
            setSocialMediaAccountTypeList(res.data?.data || []);
        })

        if (props.isUserLoaded && props.partnerId && props.partnerId !== Guid.EMPTY) {
            getPartnerSocialMediaAccountsByPartnerId.refetch().then(res => {
                setSocialMediaAccounts(res.data?.data || []);
                setIsSocialMediaAccountDataLoaded(true);
            })
        }
    }, [props.partnerId, props.isUserLoaded])

    function addSocialMediaAccount() {
        setAccountName("");
        setSocialMediaAccountTypeId(0);
        setIsEditOrAdd(true);
        setIsAddEnabled(false);
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        setAccountName("");
        setSocialMediaAccountTypeId(0);
        setCreatedByUserId(Guid.EMPTY);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        setIsEditOrAdd(false);
        setIsAddEnabled(true);
    }

    function editAccount(partnerAccountId: string) {
        getPartnerSocialMediaAccount.mutateAsync({ partnerAccountId }).then(res => {
            setSocialMediaAccountId(res.data.id);
            setAccountName(res.data.accountIdentifier);
            setSocialMediaAccountTypeId(res.data.socialMediaAccountTypeId);
            setIsActive(res.data.isActive);
            setCreatedByUserId(res.data.createdByUserId);
            setCreatedDate(new Date(res.data.createdDate));
            setLastUpdatedDate(new Date(res.data.lastUpdatedDate));
            setIsEditOrAdd(true);
            setIsAddEnabled(false);
        })
    }

    function removeAccount(accountId: string, accountName: string) {
        if (!window.confirm("Please confirm that you want to remove social media account with name: '" + accountName + "' as a social media account from this Partner?")) return;
        else {
            deletePartnerSocialMediaAccountById.mutateAsync({ accountId }).then(() => {
                setIsSocialMediaAccountDataLoaded(false);
                getPartnerSocialMediaAccountsByPartnerId.refetch().then(res => {
                    setSocialMediaAccounts(res.data?.data || []);
                    setIsSocialMediaAccountDataLoaded(true);
                })
            });
        }
    }

    async function handleSave(event: any) {
        event.preventDefault();

        if (!isSaveEnabled) return;
        setIsSaveEnabled(false);

        const body = new PartnerSocialMediaAccountData();
        body.id = socialMediaAccountId;
        body.partnerId = props.partnerId;
        body.accountIdentifier = accountName;
        body.isActive = isActive;
        body.socialMediaAccountTypeId = socialMediaAccountTypeId ?? 0;
        body.createdByUserId = createdByUserId;

        if (socialMediaAccountId === Guid.EMPTY) await createPartnerSocialMediaAccount.mutateAsync(body);
        else await updatePartnerSocialMediaAccount.mutateAsync(body);

        setIsEditOrAdd(false);
        setIsSocialMediaAccountDataLoaded(false);
        getPartnerSocialMediaAccountsByPartnerId.refetch().then(res => {
            setSocialMediaAccounts(res.data?.data || []);
            setIsSocialMediaAccountDataLoaded(true);
            setIsEditOrAdd(false);
            setIsAddEnabled(true);
        });
    }

    React.useEffect(() => {
        if (accountName === "" ||
            accountNameErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }, [accountName, accountNameErrors]);

    function handleAccountNameChanged(val: string) {
        if (val === "") {
            setAccountNameErrors("Name cannot be blank.");
        }
        else {
            setAccountNameErrors("");
            setAccountName(val);
        }
    }

    function handleIsActiveChanged(active: boolean) {
        setIsActive(active);
    }

    function selectSocialMediaAccountType(val: string) {
        setSocialMediaAccountTypeId(parseInt(val));
    }

    function renderSocialMediaAccountNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.SocialMediaAccountName}</Tooltip>
    }

    function renderSocialMediaAccountTypeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.SocialMediaAccountType}</Tooltip>
    }

    function renderIsActiveToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.SocialMediaAccountIsActive}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerSocialMediaAccountCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerSocialMediaAccountLastUpdatedDate}</Tooltip>
    }

    const accountActionDropdownList = (accountId: string, accountName: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => editAccount(accountId)}><Pencil />Edit Document</Dropdown.Item>
                <Dropdown.Item onClick={() => removeAccount(accountId, accountName)}><XSquare />Remove Document</Dropdown.Item>
            </>
        )
    }

    function renderPartnerSocialMediaAccountsTable(accounts: PartnerSocialMediaAccountData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Partner Social Media Accounts</h2>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Account Name</th>
                            <th>Account Type</th>
                            <th>Is Active</th>
                        </tr>
                    </thead>
                    <tbody>
                        {accounts.map(account =>
                            <tr key={account.id.toString()}>
                                <td>{account.accountIdentifier}</td>
                                <td>{getSocialMediaAccountType(socialMediaAccountTypeList, account.socialMediaAccountTypeId)}</td>
                                <td>{account.isActive === true ? "Yes" : "No"} </td>
                                <td className="btn py-0">
                                    <Dropdown role="menuitem">
                                        <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                        <Dropdown.Menu id="share-menu">
                                            {accountActionDropdownList(account.id, account.accountIdentifier)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button disabled={!isAddEnabled} className="action" onClick={() => addSocialMediaAccount()}>Add SocialMediaAccount</Button>
            </div>
        );
    }

    function renderAddPartnerSocialMediaAccount() {
        return (
            <div>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderSocialMediaAccountNameToolTip}>
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="AccountName">AccountName</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="username" defaultValue={accountName} onChange={val => handleAccountNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderSocialMediaAccountTypeToolTip}>
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="SocialMediaAccountType">Social Media Account Type</Form.Label>
                                </OverlayTrigger>
                                <div>
                                    <select data-val="true" name="socialMediaAccountTypeId" defaultValue={socialMediaAccountTypeId} onChange={(val) => selectSocialMediaAccountType(val.target.value)} required>
                                        <option value="">-- Select Media Type --</option>
                                        {socialMediaAccountTypeList.map(type =>
                                            <option key={type.id} value={type.id}>{type.name}</option>
                                        )}
                                    </select>
                                </div>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderIsActiveToolTip}>
                                    <ToggleButton
                                        type="checkbox"
                                        variant="outline-dark"
                                        checked={isActive}
                                        value="1"
                                        onChange={(e) => handleIsActiveChanged(e.currentTarget.checked)}
                                    >
                                        Is Active
                                    </ToggleButton>
                                </OverlayTrigger >
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Group className="form-group">
                        <Button disabled={!isSaveEnabled} type="submit" className="action btn-default">Save</Button>
                        <Button className="action" onClick={(e: any) => handleCancel(e)}>Cancel</Button>
                    </Form.Group >
                    <Form.Group className="form-group">
                        <Col>
                            <OverlayTrigger placement="top" overlay={renderCreatedDateToolTip}>
                                <Form.Label className="control-label font-weight-bold h5">Created Date</Form.Label>
                            </OverlayTrigger>
                            <Form.Group>
                                <Form.Control type="text" disabled defaultValue={createdDate ? createdDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                <Form.Label className="control-label font-weight-bold h5">Last Updated Date</Form.Label>
                            </OverlayTrigger>
                            <Form.Group>
                                <Form.Control type="text" disabled defaultValue={lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                    </Form.Group >
                </Form>
            </div>
        );
    }

    return (
        <Container>
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={4} className="d-flex">
                    <div className="bg-white py-2 px-5 shadow-sm rounded">
                        <h2 className="color-primary mt-4 mb-5">Edit Partner Social Media Accounts</h2>
                        <p>This page allows you to add a list of social media accounts you would like to have tagged when you approve a partnership request to both help spread the word about what TrashMob.eco users are
                            doing within your community, and how your organization is helping your community. This feature is still in development, but adding the information when you set things up now will help
                            when this feature fully launches.
                        </p>
                    </div>
                </Col>
                <Col lg={8}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        {props.partnerId === Guid.EMPTY && <p> <em>Partner must be created first.</em></p>}
                        {!isSocialMediaAccountsDataLoaded && props.partnerId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                        {isSocialMediaAccountsDataLoaded && renderPartnerSocialMediaAccountsTable(socialMediaAccounts)}
                        {isEditOrAdd && renderAddPartnerSocialMediaAccount()}
                    </div>
                </Col>
            </Row>
        </Container >
    );
}
