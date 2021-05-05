from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm
from fastapi import FastAPI, Depends, HTTPException, status, Response
from pydantic import BaseModel, Field, schema
from typing import Optional, Any
from itertools import count
import os


oauth2_scheme = OAuth2PasswordBearer(tokenUrl="token")
os.system("cls")
app = FastAPI()
COUNTER = count()
STORAGE = []
IDs = set()

fake_users_db = {
    "johndoe": {
        "username": "johndoe",
        "full_name": "John Doe",
        "email": "johndoe@example.com",
        "hashed_password": "fakehashedsecret",
        "disabled": False,
    },
    "alice": {
        "username": "alice",
        "full_name": "Alice Wonderson",
        "email": "alice@example.com",
        "hashed_password": "fakehashedsecret2",
        "disabled": True,
    },
}


def fake_hash_password(password: str):
    return "fakehashed" + password


def get_item(item_id):
    global STORAGE, IDs
    if item_id in IDs:
        return next(filter(lambda x: x.id == item_id, STORAGE))
    return False


class FrontItem(BaseModel):
    field1: str
    field2: str
    field3: Optional[bool]


class BackItem(BaseModel):
    id: int    # len(STORAGE)
    field1: str
    field2: str
    field3: Optional[bool]


class User(BaseModel):
    username: str
    email: Optional[str] = None
    full_name: Optional[str] = None
    disabled: Optional[bool] = None


class UserInDB(User):
    hashed_password: str


def get_user(db, username: str):
    if username in db:
        user_dict = db[username]
        return UserInDB(**user_dict)


def fake_decode_token(token):
    # This doesn't provide any security at all
    # Check the next version
    user = get_user(fake_users_db, token)
    return user


def get_current_user(token: str = Depends(oauth2_scheme)):
    user = fake_decode_token(token)
    if not user:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid authentication credentials",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return user


def get_current_active_user(current_user: User = Depends(get_current_user)):
    if current_user.disabled:
        raise HTTPException(status_code=400, detail="Inactive user")
    return current_user


@app.post("/token")
def login(form_data: OAuth2PasswordRequestForm = Depends()):
    user_dict = fake_users_db.get(form_data.username)
    if not user_dict:
        raise HTTPException(status_code=400, detail="Incorrect username or password")
    user = UserInDB(**user_dict)
    hashed_password = fake_hash_password(form_data.password)
    if not hashed_password == user.hashed_password:
        raise HTTPException(status_code=400, detail="Incorrect username or password")

    return {"access_token": user.username, "token_type": "bearer"}


@app.get("/users/me")
def read_users_me(current_user: User = Depends(get_current_active_user)):
    return current_user


@app.get('/')
def info(token: str = Depends(oauth2_scheme)):
    global STORAGE
    content_info = f'{len(STORAGE)} items' if STORAGE else 'empty'
    return {'token': token,
            'storage': content_info}


@app.post('/send')
def send(item: FrontItem, token: str = Depends(oauth2_scheme)):
    global STORAGE, IDs
    STORAGE.append(
        BackItem(id=next(COUNTER), field1=item.field1, field2=item.field2, field3=item.field3)
    )
    IDs.add(STORAGE[-1].id)
    return {'status': 'SUCCESSFUL',
            'operation': 'post',
            'item_id': STORAGE[-1].id}


@app.get('/{_id}')
def show(_id: int, token: str = Depends(oauth2_scheme)):
    target = get_item(_id)
    if target:
        return target.dict()
    return {'status': 'FAILED',
            'operation': 'get',
            'message': f'no item with id {_id}'}


@app.put('/update')
def update(item: BackItem, token: str = Depends(oauth2_scheme)):
    global STORAGE, IDs
    target = get_item(item.id)
    if target:
        idx = STORAGE.index(target)
        tmp = STORAGE[idx]
        tmp.field1, tmp.field2, tmp.field3 = item.field1, item.field2, item.field3
        return STORAGE[idx].dict()
    STORAGE.append(
        BackItem(id=item.id, field1=item.field1, field2=item.field2, field3=item.field3)
    )
    IDs.add(item.id)
    return {'status': 'SUCCESSFUL',
            'operation': 'put',
            'item_id': item.id}


@app.delete('/delete/{_id}')
def delete(_id: int, token: str = Depends(oauth2_scheme)):
    global STORAGE, IDs
    target = get_item(_id)
    if target:
        STORAGE.remove(target)
        IDs.remove(_id)
        return {'status': 'SUCCESSFUL',
                'operation': 'delete',
                'item_id': _id}
    return {'status': 'FAILED',
            'operation': 'delete',
            'message': f'no item with id {_id}'}


@app.post("/cookie-and-object/")
def create_cookie(response: Response):
    response.set_cookie(key="fakesession", value="fake-cookie-session-value")
    return {"message": "Come to the dark side, we have cookies"}
